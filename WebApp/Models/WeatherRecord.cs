using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class WeatherRecord
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string? Time { get; set; }
        public double Temperature { get; set; }
        public int RelativeHumidity { get; set; }
        public double DewPoint { get; set; }
        public double AtmosphericPressure { get; set; }
        public string? WindDirection { get; set; }
        public double WindSpeed { get; set; }
        public int Cloudiness { get; set; }
        public int CloudHeight { get; set; }
        public double? Visibility { get; set; }
        public string? WeatherPhenomenon { get; set; }
    }

}
