using System.ComponentModel.DataAnnotations.Schema;

namespace Test_WeatherApp_Server.Models
{
    // Main object of DB
    // Strings for each entry was chosen because
    // Sometimes numeric elements were written as strings
    // And because currently we only need to show information to user (no futher proccessing of data)
    [Table("WeatherForecasts")]
    public class WeatherForecast
    {
        public Guid Id { get; set; }
        public string? Day { get; set; }
        public string? Time { get; set; }
        public string? Temprature { get; set; }
        public string? Humidity { get; set; }
        public string? DewPoint { get; set; }
        public string? Pressure { get; set; }
        public string? WindDirection { get; set; }
        public string? WindSpeed { get; set; }
        public string? Cloudy { get; set; }
        public string? CloudBase { get; set; }
        public string? HorizontalVisability { get; set; }
        public string? WeatherConditions { get; set; }

        [ForeignKey(nameof(MonthId))]
        public Guid MonthId { get; set; }
        public Month? Month { get; set; }
    }
}
