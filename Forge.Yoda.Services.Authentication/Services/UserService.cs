using Forge.Security.Jwt.Shared.Service;
using Forge.Security.Jwt.Shared.Service.Models;
using Forge.Yoda.Services.Authentication.Codes;
using Forge.Yoda.Services.Authentication.Database;
using Forge.Yoda.Services.Authentication.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Forge.Yoda.Services.Authentication.Services
{

    public class UserService : IUserService
    {

        private readonly IConfiguration _configuration;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IJwtManagementService _jwtAuthManager;

        public UserService(IConfiguration configuration, SignInManager<User> signInManager, IJwtManagementService jwtAuthManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _userManager = signInManager.UserManager;
            _jwtAuthManager = jwtAuthManager;
        }

        public async Task<User> FindUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<LoginResult> Login(string username, string password, IEnumerable<JwtKeyValuePair> secondaryKeys)
        {
            await _signInManager.SignOutAsync();

            LoginResult result = new LoginResult();

            bool isExist = false;
            bool isAccountDisabled = false;
            using (DatabaseContext db = DatabaseContext.Create())
            {
                InititalizationAtStartup.IsUserAccountDisabled(db, username, out isExist, out isAccountDisabled);
            }

            if (isExist && !isAccountDisabled)
            {
                result.LoginResponse = new JwtTokenResult();

                User user = await _userManager.FindByNameAsync(username);
                SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, password, false, false);
                result.Succeeded = signInResult.Succeeded;
                result.RequiresTwoFactor = signInResult.RequiresTwoFactor;
                result.IsLockedOut = signInResult.IsLockedOut;
                result.IsNotAllowed = signInResult.IsNotAllowed;

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Surname, user.Surname!),
                    new Claim(ClaimTypes.GivenName, user.Givenname!),
                    new Claim(ClaimTypes.Email, user.Email),
                };

                JwtTokenResult jwtResult = _jwtAuthManager.GenerateTokens(username, claims, DateTime.UtcNow, secondaryKeys);
                result.LoginResponse = jwtResult;
            }

            return result;
        }

    }

}
