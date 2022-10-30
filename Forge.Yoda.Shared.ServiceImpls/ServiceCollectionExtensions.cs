using Forge.Yoda.Shared.ServiceInterfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Forge.Yoda.Shared.ServiceImpls
{

    public static class ServiceCollectionExtensions
    {

        public static IServiceCollection AddWeatherForecastService(this IServiceCollection services, Action<WeatherForecastServiceOptions> configure)
        {
            return services
                .AddScoped<IWeatherForecastService, WeatherForecastService>()
                .Configure<WeatherForecastServiceOptions>(configureOptions =>
                {
                    configure?.Invoke(configureOptions);
                });
        }

    }

}
