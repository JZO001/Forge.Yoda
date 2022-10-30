using Forge.Security.Jwt.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using System.Text;

namespace Forge.Yoda.Services.WeatherForecastApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            JwtTokenConfiguration jwtTokenConfig = builder.Configuration.GetSection("JwtTokenConfig").Get<JwtTokenConfiguration>();
            builder.Services.AddSingleton(jwtTokenConfig);

            // add JWT bearer token authentication
            builder.Services.AddAuthentication(x =>
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

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowAnyMethod();
                });
            });

            builder.Services.AddControllers().AddNewtonsoftJson();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
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

            builder.Services.AddLogging(options => options.SetMinimumLevel(LogLevel.Trace));

            var app = builder.Build();

            ILoggerFactory loggerFactory = (ILoggerFactory)app.Services.GetService(typeof(ILoggerFactory));
            loggerFactory.AddLog4Net();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
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

            app.Run();
        }
    }
}