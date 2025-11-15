using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WeatherApp.Services
{
    public class WeatherService
    {
        private readonly string _key;
        private readonly HttpClient _http;

        public WeatherService(string key)
        {
            _key = key;
            _http = new HttpClient();
        }

        public async Task<JsonDocument> GetCurrent(string city)
        {
            var url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={_key}&units=metric";
            var res = await _http.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content);
        }

        public async Task<JsonDocument> GetHourly(string city)
        {
            var current = await GetCurrent(city);
            if (!current.RootElement.TryGetProperty("coord", out var coord)) return current;

            var lat = coord.GetProperty("lat").GetDouble();
            var lon = coord.GetProperty("lon").GetDouble();
            var url = $"https://api.openweathermap.org/data/2.5/forecast?lat={lat}&lon={lon}&appid={_key}&units=metric";
            var res = await _http.GetAsync(url);
            var content = await res.Content.ReadAsStringAsync();
            return JsonDocument.Parse(content);
        }
    }
}
