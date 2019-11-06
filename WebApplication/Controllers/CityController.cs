using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ILogger<CityController> _logger;
        private readonly List<string> _cities;

        public CityController(ILogger<CityController> logger)
        {
            _logger = logger;
            _cities = new List<string>
                      {
                          "Istanbul",
                          "Ankara",
                          "Izmir"
                      };
        }

        [HttpGet]
        public string Get()
        {
            _logger.Log(LogLevel.Information, $"Request is arrived to {nameof(CityController)}");

            var random = new Random();
            int index = random.Next(3);

            string city = _cities[index];

            _logger.Log(LogLevel.Information, $"{city} is selected");

            return city;
        }
    }
}