using Microsoft.AspNetCore.Identity;

namespace Forge.Yoda.Services.Authentication.Models
{

    public class ExternalLoginResponseViewModel
    {

        public ExternalLoginResponseViewModel()
        {
            Errors = new List<IdentityError>();
        }

        public bool IsDataValid { get; set; }

        public bool IsSecurityIssue { get; set; }

        public List<IdentityError> Errors { get; set; }

        public bool Succeeded { get; set; }

        public int UserId { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public bool Enabled { get; set; }

        public int Supplier_Number { get; set; }

        public int Customer_Number { get; set; }

    }

}
