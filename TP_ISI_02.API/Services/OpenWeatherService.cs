using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using TP_ISI_02.Domain.Interfaces;
using TP_ISI_02.Domain.Models;

namespace TP_ISI_02.API.Services
{
    public class OpenWeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "b4e56d454f9ddc1a5b66102f99f28fa2"; // Key from professor's example

        public OpenWeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherDto> GetWeatherAsync(string city)
        {
            try
            {
                var response = await _httpClient.GetAsync($"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={ApiKey}&units=metric");
                
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JObject.Parse(json);

                return new WeatherDto
                {
                    City = data["name"].ToString(),
                    Description = data["weather"][0]["description"].ToString(),
                    Temperature = double.Parse(data["main"]["temp"].ToString()),
                    Humidity = int.Parse(data["main"]["humidity"].ToString())
                };
            }
            catch (Exception)
            {
                // Log error
                return null;
            }
        }
    }
}
