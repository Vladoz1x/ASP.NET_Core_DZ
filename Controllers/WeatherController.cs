using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Nodes;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherService _svc;
        public WeatherController(WeatherService svc) => _svc = svc;

        [HttpGet("current")]
        public async Task<IActionResult> Current([FromQuery] string city)
        {
            var doc = await _svc.GetCurrent(city);
            if (doc.RootElement.TryGetProperty("cod", out var cod) && cod.ToString() == "404")
                return NotFound(new { message = "Please enter a different city" });

            var root = doc.RootElement;
            var weather = root.GetProperty("weather")[0];
            var main = root.GetProperty("main");
            var wind = root.GetProperty("wind");

            var obj = new JsonObject
            {
                ["city"] = root.GetProperty("name").GetString(),
                ["date"] = root.GetProperty("dt").GetInt64(),
                ["description"] = weather.GetProperty("description").GetString(),
                ["icon"] = weather.GetProperty("icon").GetString(),
                ["temp"] = main.GetProperty("temp").GetDecimal(),
                ["tempMin"] = main.GetProperty("temp_min").GetDecimal(),
                ["tempMax"] = main.GetProperty("temp_max").GetDecimal(),
                ["wind"] = wind.GetProperty("speed").GetDecimal()
            };
            return Ok(obj);
        }

        [HttpGet("hourly")]
        public async Task<IActionResult> Hourly([FromQuery] string city)
        {
            var doc = await _svc.GetHourly(city);
            if (doc.RootElement.TryGetProperty("cod", out var cod) && cod.ToString() == "404")
                return NotFound(new { message = "Please enter a different city" });

            var listEl = doc.RootElement.GetProperty("list");
            var arr = new JsonArray();
            foreach (var x in listEl.EnumerateArray().Take(8))
            {
                var w = x.GetProperty("weather")[0];
                var obj = new JsonObject
                {
                    ["time"] = x.GetProperty("dt").GetInt64(),
                    ["description"] = w.GetProperty("description").GetString(),
                    ["icon"] = w.GetProperty("icon").GetString(),
                    ["temp"] = x.GetProperty("main").GetProperty("temp").GetDecimal(),
                    ["wind"] = x.GetProperty("wind").GetProperty("speed").GetDecimal()
                };
                arr.Add(obj);
            }
            return Ok(arr);
        }
    }
}
