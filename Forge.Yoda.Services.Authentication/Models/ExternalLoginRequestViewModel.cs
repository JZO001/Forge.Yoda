using System.ComponentModel.DataAnnotations;

namespace Forge.Yoda.Services.Authentication.Models
{

    [Serializable]
    public class ExternalLoginRequestViewModel
    {

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

    }

}
