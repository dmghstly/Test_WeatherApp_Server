using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Test_WeatherApp_Server.Interfaces;
using Test_WeatherApp_Server.Models;

namespace Test_WeatherApp_Server.Controllers
{
    // This is basic REST API controller
    // It has retrieval of information about Years, Months and Weather Forecast
    // Also this controller has methods to work with files
    [ApiController]
    [Route("WeatherForecastAPI")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IWeatherForecastFileHandler _fileHandelr;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherForecastService _weatherForecastService;

        public WeatherForecastController(IWeatherForecastFileHandler fileHandelr,
            ILogger<WeatherForecastController> logger,
            IWeatherForecastService weatherForecastService)
        {
            _fileHandelr = fileHandelr;
            _logger = logger;
            _weatherForecastService = weatherForecastService;
        }

        [HttpGet("/GetMonths")]
        public async Task<IEnumerable<Month>> GetMonths()
        {
            _logger.LogInformation($"Information about months was retrieved from database");

            return await _weatherForecastService.GetMonths();
        }

        [HttpGet("/GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecast(Guid monthId)
        {
            _logger.LogInformation($"Information about weather forecast with {monthId} was retrieved from database");

            return await _weatherForecastService.GetForecasts(monthId);
        }

        [HttpPost("/UploadMultipleFiles")]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IEnumerable<StatusUploadFile>> UploadMultipleFiles(IEnumerable<IFormFile> files)
        {
            var statuses = new List<StatusUploadFile>();

            foreach (var item in files)
            {
                var status = await _fileHandelr.ReadWeatherForecast(item);

                statuses.Add(status);
            }

            return statuses;
        }
    }
}
