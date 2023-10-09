using Microsoft.EntityFrameworkCore;
using Test_WeatherApp_Server.Models;

namespace Test_WeatherApp_Server.Context
{
    public class WeatherForecastDbContext : DbContext
    {
        public DbSet<Month> Months { get; set; }
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }

        public WeatherForecastDbContext(DbContextOptions<WeatherForecastDbContext> options) : base(options)
        {
        }
    }
}
