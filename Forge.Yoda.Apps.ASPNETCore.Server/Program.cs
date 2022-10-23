using Microsoft.AspNetCore.ResponseCompression;
using System.Net;

namespace Forge.Yoda.Apps.ASPNETCore.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            /*
            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Listen(IPAddress.Parse("192.168.0.100"), 7166, listenOptions =>
                {
                    //listenOptions.UseHttps("Forge.Yoda.pfx", "Passw0rd");
                    listenOptions.UseHttps(System.Security.Cryptography.X509Certificates.StoreName.My, "localhost");
                });
                serverOptions.ListenAnyIP(7166, listenOptions =>
                {
                    //listenOptions.UseHttps("Forge.Yoda.pfx", "Passw0rd");
                    listenOptions.UseHttps(System.Security.Cryptography.X509Certificates.StoreName.My, "localhost");
                });
                serverOptions.ListenLocalhost(7166, listenOptions =>
                {
                    listenOptions.UseHttps(System.Security.Cryptography.X509Certificates.StoreName.My, "localhost");
                });

                serverOptions.ListenAnyIP(5166);
                serverOptions.Listen(IPAddress.Parse("192.168.0.100"), 5166);
                serverOptions.ListenLocalhost(5166);
            });
            */

            // Add services to the container.

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();


            app.MapRazorPages();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}