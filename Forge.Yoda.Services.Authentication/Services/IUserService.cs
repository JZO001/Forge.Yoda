using Forge.Security.Jwt.Shared.Service.Models;
using Forge.Yoda.Services.Authentication.Database;
using Forge.Yoda.Services.Authentication.Models;

namespace Forge.Yoda.Services.Authentication.Services
{

    public interface IUserService
    {

        Task<LoginResult> Login(string userName, string password, IEnumerable<JwtKeyValuePair> secondaryKeys);

        Task<User> FindUserById(string userId);

    }

}
