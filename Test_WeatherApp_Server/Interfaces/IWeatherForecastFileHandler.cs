using Test_WeatherApp_Server.Models;

namespace Test_WeatherApp_Server.Interfaces
{
    public interface IWeatherForecastFileHandler
    {
        Task<StatusUploadFile> ReadWeatherForecast(IFormFile file);
    }
}
