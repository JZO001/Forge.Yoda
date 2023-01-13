using Forge.Security.Jwt.Shared.Client.Api;
using Forge.Yoda.Shared.Models;
using Forge.Yoda.Shared.ServiceInterfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Forge.Yoda.Shared.ServiceImpls
{

    public class WeatherForecastService : IWeatherForecastService
    {

        private readonly ILogger<WeatherForecastService> _logger;
        private readonly ITokenizedApiCommunicationService _apiCommunicationService;
        private readonly WeatherForecastServiceOptions _options;

        public WeatherForecastService(ILogger<WeatherForecastService> logger, 
            ITokenizedApiCommunicationService apiCommunicationService, 
            IOptions<WeatherForecastServiceOptions> options)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (apiCommunicationService == null) throw new ArgumentNullException(nameof(apiCommunicationService));
            if (options == null) throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _apiCommunicationService = apiCommunicationService;
            _options = options.Value;
        }

        public WeatherForecastService(ILogger<WeatherForecastService> logger,
            ITokenizedApiCommunicationService apiCommunicationService,
            WeatherForecastServiceOptions options)
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (apiCommunicationService == null) throw new ArgumentNullException(nameof(apiCommunicationService));
            if (options == null) throw new ArgumentNullException(nameof(options));
            _logger = logger;
            _apiCommunicationService = apiCommunicationService;
            _options = options;
        }

        public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            WeatherForecast[] result = null;
            try
            {
                result = await _apiCommunicationService.GetAsync<WeatherForecast[]>($"{_options.BaseAddress}{_options.Uri}", CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result = new WeatherForecast[0];
            }
            return result;
        }

    }

}
