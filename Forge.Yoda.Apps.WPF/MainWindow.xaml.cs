using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Windows;
using Forge.Security.Jwt.Client.Storage.Browser;
using Forge.Wasm.BrowserStorages.Services.LocalStorage;
using Forge.Security.Jwt.Client;
using Forge.Security.Jwt.Shared.Service.Models;
using Forge.Yoda.Shared.Models;
using Forge.Yoda.Shared.ServiceImpls;
using Forge.Yoda.Shared.ServiceInterfaces;
using Forge.Yoda.Shared.UI.Models;

namespace Forge.Yoda.Apps.WPF
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            Func<HttpMessageHandler> GetInsecureHandler = delegate ()
            {
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (cert != null && cert.Issuer.Equals("CN=localhost"))
                        return true;
                    return errors == System.Net.Security.SslPolicyErrors.None;
                };
                return handler;
            };

            ServiceCollection services = new ServiceCollection();

            services.AddLogging(logging => logging.SetMinimumLevel(LogLevel.Trace));

            services.AddOptions();

            services.AddAuthorizationCore();

            services.AddWpfBlazorWebView();
#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
#endif

            services.AddForgeLocalStorage();

            services.AddForgeJwtClientAuthenticationCore((options) =>
            {
#if DEBUG
                // For development
                options.BaseAddress = "https://localhost:7253/";
#else
                // TODO: change it to the live address
                options.BaseAddress = "https://auth.yourdomain.com/";
#endif
                options.RefreshTokenBeforeExpirationInMilliseconds = 50000;
                options.SecondaryKeys.Add(new JwtKeyValuePair(Consts.DEVICE_ID, "eb11812d-fb1b-4ec9-95d3-c9f9f9832a2a"));
                options.HttpMessageHandlerFactory = GetInsecureHandler;
            });

            services.AddForgeJwtClientAuthenticationCoreWithLocalStorage();

            services.AddScoped<UserContext>();

            // business services goes here...
            services.AddWeatherForecastService(config => {
#if DEBUG
                // For development
                config.BaseAddress = "https://localhost:7067/";
#else
                // TODO: change it to the live address
                config.BaseAddress = "https://weatherservice.yourdomain.com/";
#endif
            });


            ServiceProvider serviceProvider = services.BuildServiceProvider();

            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>()!;
            loggerFactory.AddLog4Net();

            Resources.Add("services", serviceProvider);
        }

    }

}
