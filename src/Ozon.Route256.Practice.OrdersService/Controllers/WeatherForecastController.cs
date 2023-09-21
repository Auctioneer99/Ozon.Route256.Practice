using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Practice.OrdersService.Services;

namespace Ozon.Route256.Practice.OrdersService.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly WeatherForecastService _service;
    
    public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherForecastService service)
    {
        _logger = logger;
        _service = service;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        return _service.Get();
    }
}