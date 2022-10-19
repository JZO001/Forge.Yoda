using Forge.Security.Jwt.Shared.Service.Models;
using Newtonsoft.Json;

namespace Forge.Yoda.Services.Authentication.Models
{

    [Serializable]
    public class LogoutRequestViewModel
    {

        [JsonProperty("secondaryKeys")]
        public List<JwtKeyValuePair> SecondaryKeys { get; set; } = new List<JwtKeyValuePair>();

    }

}
