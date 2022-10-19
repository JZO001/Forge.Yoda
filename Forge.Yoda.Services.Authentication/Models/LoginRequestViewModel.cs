using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Forge.Yoda.Services.Authentication.Models
{

    [Serializable]
    public class LoginRequestViewModel : LogoutRequestViewModel
    {

        [JsonProperty("username")]
        [Required]
        public string? Username { get; set; }

        [JsonProperty("password")]
        [Required]
        public string? Password { get; set; }

    }

}
