using Forge.Yoda.Services.Authentication.Database;
using Microsoft.AspNetCore.Identity;

namespace Forge.Yoda.Services.Authentication.Codes
{

    public class InititalizationAtStartup
    {

        public const string USER_ADMIN_NAME = "Admin";

        private ILogger<InititalizationAtStartup> mLogger;
        private UserManager<User> mUserManager;

        public InititalizationAtStartup(ILogger<InititalizationAtStartup> logger, UserManager<User> userManager)
        {
            mLogger = logger;
            mUserManager = userManager;
        }

        public async Task Initialize(bool isDev)
        {
            bool isAdminExist = false;
            bool isAdminAccountDisabled = false;

            using (DatabaseContext db = DatabaseContext.Create())
            {
                IsUserAccountDisabled(db, USER_ADMIN_NAME, out isAdminExist, out isAdminAccountDisabled);
            }

            User adminUser = null;
            if (!isAdminExist)
            {
                adminUser = new User();
                adminUser.UserName = USER_ADMIN_NAME;
                adminUser.LockoutEnabled = false;
                adminUser.Surname = "BuiltIn";
                adminUser.Givenname = "Admin";
                adminUser.Email = "noreply@yourdomain.com";
                adminUser.EmailConfirmed = true;
                adminUser.Role = Roles.ROLE_ADMIN;
                await mUserManager.CreateAsync(adminUser, "Passw0rd12345");
            }
        }

        public static void IsUserAccountDisabled(DatabaseContext db, string userName, out bool isExist, out bool isAccountDisabled)
        {
            isExist = false;
            isAccountDisabled = false;

            User user = db.Users.SingleOrDefault(u => u.UserName == userName);
            if (user != null)
            {
                isExist = true;
                isAccountDisabled = user.IsAccountDisabled;
            }
        }

    }

}
