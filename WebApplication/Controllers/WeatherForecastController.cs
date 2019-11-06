using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication.Proxies;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
                                                     {
                                                         "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
                                                     };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ICityClient _cityClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICityClient cityClient)
        {
            _logger = logger;
            _cityClient = cityClient;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.Log(LogLevel.Information, $"Request is arrived to {nameof(WeatherForecastController)}");

            string city = await _cityClient.Get();

            var rng = new Random();
            var response = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                                                                  {
                                                                      City = city,
                                                                      Date = DateTime.Now.AddDays(index),
                                                                      TemperatureC = rng.Next(-20, 55),
                                                                      Summary = Summaries[rng.Next(Summaries.Length)]
                                                                  })
                                     .ToArray();

            _logger.Log(LogLevel.Information, $"Response is ready to return from {nameof(WeatherForecastController)}");

            return response;
        }
    }
}