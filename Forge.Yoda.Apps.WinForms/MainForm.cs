using Forge.Security.Jwt.Client;
using Forge.Security.Jwt.Client.Storage.Browser;
using Forge.Security.Jwt.Shared.Client.Models;
using Forge.Security.Jwt.Shared.Service.Models;
using Forge.Wasm.BrowserStorages.Services.LocalStorage;
using Forge.Yoda.Shared.Models;
using Forge.Yoda.Shared.ServiceImpls;
using Forge.Yoda.Shared.ServiceInterfaces;
using Forge.Yoda.Shared.UI.Models;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Forge.Yoda.Shared.UI;

namespace Forge.Yoda.Apps.WinForms
{
    public partial class MainForm : Form
    {

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            ServiceCollection services = new ServiceCollection();

            Func<HttpMessageHandler> GetInsecureHandler = delegate ()
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (cert != null && cert.Issuer.Equals("CN=localhost")) return true;
                    return errors == System.Net.Security.SslPolicyErrors.None;
                };
                return handler;
            };

            services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace));

            services.AddOptions();

            services.AddAuthorizationCore();

            services.AddWindowsFormsBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

            services.AddForgeLocalStorage();

            services.AddForgeJwtClientAuthenticationCore((JwtClientAuthenticationCoreOptions options) => {
#if DEBUG
                // For development
                options.BaseAddress = "https://localhost:7253/";
#else
                // TODO: change it to the live address
                options.BaseAddress = "https://auth.yourdomain.com";
#endif
                options.RefreshTokenBeforeExpirationInMilliseconds = 50000;
                options.SecondaryKeys.Add(new JwtKeyValuePair(Consts.DEVICE_ID, "7010c030-6a2c-4dc5-86a3-2a9702baa7b3"));
                options.HttpMessageHandlerFactory = GetInsecureHandler;
            });

            services.AddForgeJwtClientAuthenticationCoreWithLocalStorage();

            services.AddScoped<UserContext>();

            // business services goes here...
            //services.AddTransient(sp => new HttpClient(GetInsecureHandler()) { BaseAddress = new Uri("https://localhost:7067/") });
            //services.AddScoped(typeof(IWeatherForecastService), typeof(WeatherForecastService));
            services.AddWeatherForecastService(config => {
#if DEBUG
                // For development
                config.BaseAddress = "https://localhost:7067/";
#else
                // TODO: change it to the live address
                config.BaseAddress = "https://weatherservice.yourdomain.com";
#endif
            });

            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddLog4Net();

            blazorWebView.HostPage = "wwwroot\\index.html";
            blazorWebView.Services = serviceProvider;
            blazorWebView.RootComponents.Add<App>("#app");
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

    }
}