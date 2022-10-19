using Forge.Security.Jwt.Shared.Client.Services;
using Forge.Security.Jwt.Shared.Service.Models;

namespace Forge.Yoda.Shared.UI.Models
{

    public class AuthResponse : IAuthenticationResponse
    {

        public string? AccessToken { get; set; } = string.Empty;

        public string? RefreshToken { get; set; } = string.Empty;

        public DateTime RefreshTokenExpireAt { get; set; } = DateTime.MinValue;

    }

}
