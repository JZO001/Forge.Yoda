using Forge.Yoda.Shared.Models;
using Forge.Yoda.Shared.ServiceInterfaces;
using System.Net.Http.Json;

namespace Forge.Yoda.Shared.ServiceImpls
{
    public class WeatherForecastService : IWeatherForecastService
    {

        private HttpClient _httpClient = null;

        public WeatherForecastService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            return await _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast");
        }

    }
}
