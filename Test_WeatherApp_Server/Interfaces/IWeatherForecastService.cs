using Test_WeatherApp_Server.Models;

namespace Test_WeatherApp_Server.Interfaces
{
    public interface IWeatherForecastService
    {
        Task<IEnumerable<Month>> GetMonths();
        Task<IEnumerable<WeatherForecast>> GetForecasts(Guid monthId);
    }
}
