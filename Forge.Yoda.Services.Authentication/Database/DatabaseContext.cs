using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forge.Yoda.Services.Authentication.Database
{

    public class DatabaseContext : IdentityDbContext<User>
    {

        public DatabaseContext() : base(CreateOptions())
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public static string DefaultConnectionString { get; set; } = "Data Source=.\\SQLEXPRESS2019;Initial Catalog=ForgeYodaAuth;Integrated Security=True";

        public static DatabaseContext Create()
        {
            return new DatabaseContext(CreateOptions());
        }

        private static DbContextOptions<DatabaseContext> CreateOptions()
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer(DefaultConnectionString);
            return optionsBuilder.Options;
        }

    }

}
