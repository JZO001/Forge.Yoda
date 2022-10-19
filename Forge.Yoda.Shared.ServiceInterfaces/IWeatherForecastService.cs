using Forge.Yoda.Shared.Models;

namespace Forge.Yoda.Shared.ServiceInterfaces
{

    public interface IWeatherForecastService
    {

        Task<WeatherForecast[]> GetForecastAsync(DateTime startDate);

    }

}