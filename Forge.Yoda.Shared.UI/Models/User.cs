using Forge.Security.Jwt.Shared.Client.Models;
using System.Security.Claims;

namespace Forge.Yoda.Shared.UI.Models
{

    [Serializable]
    public class User
    {

        public User()
        {
        }

        public User(ParsedTokenData parsedTokenData)
        {
            if (parsedTokenData == null) throw new ArgumentNullException(nameof(parsedTokenData));

            UserId = parsedTokenData.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).Select(x => x.Value).FirstOrDefault();
            Email = parsedTokenData.Claims.Where(x => x.Type == ClaimTypes.Email).Select(x => x.Value).FirstOrDefault();
            Surname = parsedTokenData.Claims.Where(x => x.Type == ClaimTypes.Surname).Select(x => x.Value).FirstOrDefault();
            GivenName = parsedTokenData.Claims.Where(x => x.Type == ClaimTypes.GivenName).Select(x => x.Value).FirstOrDefault();
            Role = parsedTokenData.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();

            string? Supplier_NumberStr = parsedTokenData.Claims.Where(x => x.Type == "Supplier_Number").Select(x => x.Value).FirstOrDefault();

            int parsed_Number = -1;
            if (int.TryParse(Supplier_NumberStr, out parsed_Number))
            {
                Supplier_Number = parsed_Number;
            }
            if (int.TryParse(Supplier_NumberStr, out parsed_Number))
            {
                Customer_Number = parsed_Number;
            }
        }

        public string? UserId { get; protected set; }

        public string? Email { get; protected set; }

        public string? Surname { get; protected set; } // family name

        public string? GivenName { get; protected set; }

        public string? Role { get; protected set; }

        public int Supplier_Number { get; protected set; } = -1;

        public int Customer_Number { get; protected set; } = -1;

    }

}
