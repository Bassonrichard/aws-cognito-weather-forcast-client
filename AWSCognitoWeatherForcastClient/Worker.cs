using AWSCognitoWeatherForcastClient.Models;
using AWSCognitoWeatherForcastClient.Services;
using System.Text.Json;

namespace AWSCognitoWeatherForcastClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ICommonApplicationService _commonApplicationService;

        public Worker(ILogger<Worker> logger, ICommonApplicationService commonApplicationService)
        {
            _logger = logger;
            _commonApplicationService = commonApplicationService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                IEnumerable<WeatherForecast> response = await _commonApplicationService.GetWeatherForcast();
                string weather =  JsonSerializer.Serialize(response);

                _logger.LogInformation($"Weather:{weather}");

                await Task.Delay(100000, stoppingToken);
            }
        }
    }
}