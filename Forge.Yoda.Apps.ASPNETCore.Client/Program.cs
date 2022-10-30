using Forge.Security.Jwt.Client;
using Forge.Security.Jwt.Client.Storage.Browser;
using Forge.Security.Jwt.Shared.Service.Models;
using Forge.Wasm.BrowserStorages.Services.LocalStorage;
using Forge.Yoda.Shared.Models;
using Forge.Yoda.Shared.ServiceImpls;
using Forge.Yoda.Shared.ServiceInterfaces;
using Forge.Yoda.Shared.UI.Models;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Forge.Yoda.Shared.UI;
using Forge.Wasm.BrowserStorages.Services.Abstraction;

namespace Forge.Yoda.Apps.ASPNETCore.Client
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");
            builder.Logging.SetMinimumLevel(LogLevel.Trace);

            builder.Services.AddLogging();

            builder.Services.AddOptions();

            builder.Services.AddAuthorizationCore();

            builder.Services.AddForgeLocalStorage();
            //builder.Services.AddForgeSessionStorage();

            builder.Services.AddForgeJwtClientAuthenticationCore((options) =>
            {
                options.BaseAddress = "https://localhost:7253/";
                options.RefreshTokenBeforeExpirationInMilliseconds = 50000;
                options.SecondaryKeys.Add(new JwtKeyValuePair(Consts.DEVICE_ID, "2c801461-b22d-4d03-9368-4cf3154394d1"));
            });

            builder.Services.AddForgeJwtClientAuthenticationCoreWithLocalStorage();
            //builder.Services.AddForgeJwtClientAuthenticationCoreWithSessionStorage();

            //builder.Services.AddSingleton<IJSInProcessRuntime>(services => (IJSInProcessRuntime)services.GetRequiredService<IJSRuntime>());

            builder.Services.AddScoped<UserContext>();

            // business services goes here...
            //builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}:7067") });
            //builder.Services.AddScoped(typeof(IWeatherForecastService), typeof(WeatherForecastService));
            builder.Services.AddWeatherForecastService(config => {
                config.BaseAddress = "https://localhost:7067/";
            });

            await builder.Build().RunAsync();
        }
    }
}