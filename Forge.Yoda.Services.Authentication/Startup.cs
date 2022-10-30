using Forge.Security.Jwt.Service;
using Forge.Yoda.Services.Authentication.Codes;
using Forge.Yoda.Services.Authentication.Database;
using Forge.Yoda.Services.Authentication.Services;
using Forge.Security.Jwt.Service.Storage.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using System.Text;
using Forge.Yoda.Services.Authentication.Codes;

namespace Forge.Yoda.Services.Authentication
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // bind configuration to POCO
            JwtTokenConfiguration jwtTokenConfig = Configuration.GetSection("JwtTokenConfig").Get<JwtTokenConfiguration>();
            services.AddSingleton(jwtTokenConfig);

            // migrate database, if neccessary
            {
                string connectionString = DatabaseContext.DefaultConnectionString = Configuration.GetConnectionString("DefaultConnection");
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
                dbContextOptionsBuilder.UseSqlServer(connectionString);
                using (DatabaseContext context = new DatabaseContext(dbContextOptionsBuilder.Options))
                {
                    context.Database.Migrate();
                }
                services.AddDbContext<DatabaseContext>(config =>
                {
                    config.UseSqlServer(connectionString);
                    //config.UseInMemoryDatabase("Memory");
                });
            }

            // AddIdentity registers the services
            services.AddIdentity<User, IdentityRole>(config =>
            {
                config.Password.RequiredLength = 1;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Password.RequireLowercase = false;
                //config.Password.RequiredLength = 6;
                //config.Password.RequireDigit = true;
                //config.Password.RequireNonAlphanumeric = false;
                //config.Password.RequireUppercase = true;
                config.SignIn.RequireConfirmedEmail = false;
            })
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            // add JWT bearer token authentication
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtTokenConfig.ValidateIssuer,
                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidateIssuerSigningKey = jwtTokenConfig.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    ValidAudience = jwtTokenConfig.Audience,
                    ValidateAudience = jwtTokenConfig.ValidateAudience,
                    ValidateLifetime = jwtTokenConfig.ValidateLifetime,
                    ClockSkew = TimeSpan.FromMinutes(jwtTokenConfig.ClockSkewInMinutes)
                };
            });

            // add Jwt Token service
            services.AddForgeJwtServerAuthenticationCore();
            // add SqlServer storage
            services.AddForgeJwtServiceSqlServerStorage(config => {
                config.ConnectionString = Configuration.GetConnectionString("ServiceStorageConnection");
            });

            services.AddScoped<IUserService, UserService>();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod();
                });
            });

            services.AddControllers().AddNewtonsoftJson();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Access Token", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    BearerFormat = JwtBearerDefaults.AuthenticationScheme,
                    Description = "Standard Authorization header using the Bearer scheme. Example: \"Bearer {access token}\"",
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header
                });

                options.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.AddTransient<InititalizationAtStartup>();

            services.AddLogging(options => options.SetMinimumLevel(LogLevel.Trace));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IWebHostEnvironment env,
            ILoggerFactory loggerFactory,
            IHostApplicationLifetime applicationLifetime)
        {
            loggerFactory.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;

            InititalizationAtStartup initStartup = serviceProvider.GetService<InititalizationAtStartup>();
            Task.WaitAll(initStartup.Initialize(env.IsDevelopment()));

        }

    }
}
