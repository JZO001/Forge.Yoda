using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Forge.Yoda.Services.Authentication.Database
{

    public class User : IdentityUser
    {

        public User() : base()
        {
        }

        [Required]
        public string? Surname { get; set; } // family name

        [Required]
        public string? Givenname { get; set; }

        [Required]
        public bool IsAccountDisabled { get; set; }

        [Required]
        public string Role { get; set; } = Roles.ROLE_USER;

    }

}
