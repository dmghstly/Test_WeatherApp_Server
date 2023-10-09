using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Test_WeatherApp_Server.Context;
using Test_WeatherApp_Server.Controllers;
using Test_WeatherApp_Server.Interfaces;
using Test_WeatherApp_Server.Models;

namespace Test_WeatherApp_Server.Services
{
    // This service gets all necessary data from DB
    public class WeatherForecastService : IWeatherForecastService
    {
        private readonly WeatherForecastDbContext _dbContext;

        // This is a cache, to make less work with DB when getting weather forecast information
        private static ConcurrentDictionary<Guid, IEnumerable<WeatherForecast>> _weatherForecastCache = 
            new();    

        public WeatherForecastService(WeatherForecastDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Getting weather forecast by month
        public async Task<IEnumerable<WeatherForecast>> GetForecasts(Guid monthId)
        {
            // Checking cache
            if (_weatherForecastCache.TryGetValue(monthId, out var weatherForecast))
            {
                return weatherForecast;
            }

            // Updating cache if necessary
            else
            {
                var weatherForecastFromDB = await _dbContext.Set<WeatherForecast>().Where(wf => wf.MonthId == monthId).ToListAsync();

                _weatherForecastCache.TryAdd(monthId, weatherForecastFromDB);

                return weatherForecastFromDB;
            }
        }

        // Getting information of month within a year
        public async Task<IEnumerable<Month>> GetMonths()
        {
            return await _dbContext.Set<Month>().ToListAsync();
        }
    }
}
