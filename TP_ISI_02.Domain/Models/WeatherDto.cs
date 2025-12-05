using System;

namespace TP_ISI_02.Domain.Models
{
    public class WeatherDto
    {
        public string City { get; set; }
        public string Description { get; set; }
        public double Temperature { get; set; }
        public int Humidity { get; set; }
    }
}
