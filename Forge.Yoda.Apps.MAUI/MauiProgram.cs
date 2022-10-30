using Forge.Security.Jwt.Client;
using Forge.Security.Jwt.Shared.Service.Models;
using Forge.Wasm.BrowserStorages.Services.LocalStorage;
using Forge.Security.Jwt.Client.Storage.Browser;
using Forge.Yoda.Shared.Models;
using Forge.Yoda.Shared.ServiceImpls;
using Forge.Yoda.Shared.ServiceInterfaces;
using Forge.Yoda.Shared.UI.Models;
#if ANDROID
using Forge.Yoda.Apps.MAUI.Platforms.Android;
#endif

namespace Forge.Yoda.Apps.MAUI
{
    public static class MauiProgram
    {

#if DEBUG

        private static HttpMessageHandler GetLocalhostHandler()
        {
#if ANDROID
            DevelopmentAndroidMessageHandler handler = new DevelopmentAndroidMessageHandler();
#else
            HttpClientHandler handler = new HttpClientHandler();
#endif
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost")) return true;
                return errors == System.Net.Security.SslPolicyErrors.None;
            };

            return handler;
        }

#endif

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            string baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:7067" : "https://localhost:7067";
            builder.Services.AddScoped(sp => new HttpClient(GetLocalhostHandler()) { BaseAddress = new Uri(baseAddress) });
#else
            // TODO: change it to the final address
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://liveaddress.yourdomain.com") });
#endif

            builder.Services.AddOptions();

            builder.Services.AddAuthorizationCore();

            //builder.Services.AddSingleton<IJSInProcessRuntime>(services => (IJSInProcessRuntime)services.GetRequiredService<IJSRuntime>());

            builder.Services.AddForgeLocalStorage();

            builder.Services.AddForgeJwtClientAuthenticationCore((options) =>
            {
#if DEBUG
                options.BaseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "https://10.0.2.2:7253" : "https://localhost:7253";
#else
                options.BaseAddress = "https://auth.yourdomain.com";
#endif
                options.RefreshTokenBeforeExpirationInMilliseconds = 50000;
                options.SecondaryKeys.Add(new JwtKeyValuePair(Consts.DEVICE_ID, "7010c030-6a2c-4dc5-86a3-2a9702baa7b3"));
                options.HttpMessageHandlerFactory = GetLocalhostHandler;
            });

            builder.Services.AddForgeJwtClientAuthenticationCoreWithLocalStorage();

            builder.Services.AddScoped<UserContext>();

            //builder.Services.AddScoped(typeof(IWeatherForecastService), typeof(WeatherForecastService));
            builder.Services.AddWeatherForecastService(config => {
                config.BaseAddress = "https://localhost:7067/";
            });

            return builder.Build();
        }

    }

}