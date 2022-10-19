using Forge.Security.Jwt.Shared.Client.Models;
using System.ComponentModel.DataAnnotations;

namespace Forge.Yoda.Shared.UI.Models
{

    public class AuthCredentials : AdditionalData
    {

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

    }

}
