using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test_WeatherApp_Server.Models
{
    public class Month
    {
        [Key]
        public Guid Id { get; set; }
        public string? Name { get; set; }

        public uint YearNum { get; set; }
    }
}
