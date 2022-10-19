using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using System.Net;
using Forge.Security.Jwt.Shared.Service;
using Forge.Security.Jwt.Shared.Service.Models;
using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Yoda.Services.Authentication.Models;
using Forge.Yoda.Services.Authentication.Database;
using Forge.Yoda.Services.Authentication.Services;

namespace Forge.Yoda.Services.Authentication.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private readonly IUserService _userService;
        private readonly IJwtManagementService _jwtManagementService;

        public AuthController(ILogger<AuthController> logger, IUserService userService, IJwtManagementService jwtManagementService)
        {
            _logger = logger;
            _userService = userService;
            _jwtManagementService = jwtManagementService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestViewModel request)
        {
            if (!ModelState.IsValid) return BadRequest();

            string? ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            string userAgent = GetUserAgentFromRequestHeaders();

            List<JwtKeyValuePair> keys = new();
            keys.Add(new JwtKeyValuePair("RemoteIpAddress", ip));
            keys.Add(new JwtKeyValuePair("user-agent", userAgent));

            if (request.SecondaryKeys != null && request.SecondaryKeys.Count > 0)
            {
                HashSet<string> reservedKeys = new HashSet<string>(new[] { "RemoteIpAddress", "user-agent" });
                request.SecondaryKeys.ForEach(item =>
                {
                    if (!reservedKeys.Contains(item.Key))
                    {
                        keys.Add(new JwtKeyValuePair(item.Key, item.Value));
                    }
                });
            }

            LoginResult result = await _userService.Login(request.Username, request.Password, keys);
            if (!result.Succeeded)
            {
                _logger.LogInformation($"User [{request.Username}] failed to log in the system. IsLockedOut: {result.IsLockedOut}, IsNotAllowed: {result.IsNotAllowed}, RequiresTwoFactor: {result.RequiresTwoFactor}, IP: [{ip}], User-Agent: [{userAgent}]");
                return Unauthorized();
            }

            _logger.LogInformation($"User [{request.Username}] logged in.");

            return Ok(result.LoginResponse);
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout([FromBody] LogoutRequestViewModel request)
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            List<JwtKeyValuePair> keys = new();
            keys.Add(new JwtKeyValuePair("RemoteIpAddress", Request.HttpContext.Connection.RemoteIpAddress?.ToString()));
            keys.Add(new JwtKeyValuePair("user-agent", GetUserAgentFromRequestHeaders()));

            if (request.SecondaryKeys != null && request.SecondaryKeys.Count > 0)
            {
                HashSet<string> reservedKeys = new HashSet<string>(new[] { "RemoteIpAddress", "user-agent" });
                request.SecondaryKeys.ForEach(item =>
                {
                    if (!reservedKeys.Contains(item.Key))
                    {
                        keys.Add(new JwtKeyValuePair(item.Key, item.Value));
                    }
                });
            }

            string? userName = User.Identity?.Name;
            bool result = _jwtManagementService.RemoveRefreshTokenByUserNameAndKeys(userName, keys);

            _logger.LogInformation($"User [{userName}] logged out. Result: {result}");

            return Ok(new BooleanResponse() { Result = result });
        }

        [HttpPost("validate-token")]
        [Authorize]
        public async Task<ActionResult> ValidateToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                string? userName = User.Identity?.Name;
                _logger.LogInformation($"User [{userName}] is validating JWT token.");

                string? accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

                string? ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                string userAgent = GetUserAgentFromRequestHeaders();

                List<JwtKeyValuePair> keys = new();
                keys.Add(new JwtKeyValuePair("RemoteIpAddress", ip));
                keys.Add(new JwtKeyValuePair("user-agent", userAgent));

                if (request.SecondaryKeys != null && request.SecondaryKeys.Count > 0)
                {
                    HashSet<string> reservedKeys = new HashSet<string>(new[] { "RemoteIpAddress", "user-agent" });
                    request.SecondaryKeys.ForEach(item =>
                    {
                        if (!reservedKeys.Contains(item.Key))
                        {
                            keys.Add(new JwtKeyValuePair(item.Key, item.Value));
                        }
                    });
                }

                JwtTokenValidationResultEnum jwtValidationResult = _jwtManagementService.Validate(request.RefreshTokenString, accessToken, DateTime.UtcNow, keys);

                _logger.LogInformation($"User [{userName}] has validated JWT token. Result: {jwtValidationResult}");

                return Ok(new BooleanResponse() { Result = jwtValidationResult == JwtTokenValidationResultEnum.Valid });
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult> RefreshToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid) return BadRequest();

            try
            {
                string? userName = User.Identity?.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                string? accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");

                string? ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                string userAgent = GetUserAgentFromRequestHeaders();

                List<JwtKeyValuePair> keys = new();
                keys.Add(new JwtKeyValuePair("RemoteIpAddress", ip));
                keys.Add(new JwtKeyValuePair("user-agent", userAgent));

                if (request.SecondaryKeys != null && request.SecondaryKeys.Count > 0)
                {
                    HashSet<string> reservedKeys = new HashSet<string>(new[] { "RemoteIpAddress", "user-agent" });
                    request.SecondaryKeys.ForEach(item =>
                    {
                        if (!reservedKeys.Contains(item.Key))
                        {
                            keys.Add(new JwtKeyValuePair(item.Key, item.Value));
                        }
                    });
                }

                JwtTokenResult jwtResult = _jwtManagementService.Refresh(request.RefreshTokenString, accessToken, DateTime.UtcNow, keys);

                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");

                using (DatabaseContext db = DatabaseContext.Create())
                {
                    User? user = db.Users.SingleOrDefault(u => u.UserName == userName);

                    if (user == null) return Unauthorized(); // redirect to the login page

                    return Ok(jwtResult);
                }
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
            catch (Exception ex)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        private string GetUserAgentFromRequestHeaders()
        {
            string result = string.Empty;
            foreach (KeyValuePair<string, StringValues> kv in Request.Headers)
            {
                if ("user-agent".Equals(kv.Key.ToLower()))
                {
                    if (kv.Value.Count > 0) result = kv.Value.ToArray()[0];
                    break;
                }
            }
            return result;
        }

    }

}
