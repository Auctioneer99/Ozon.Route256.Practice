using Ozon.Route256.Practice.OrdersService.Services;

namespace Ozon.Route256.Practice.Test;

public class Tests
{
    private WeatherForecastService _service;
    
    [SetUp]
    public void Setup()
    {
        _service = new WeatherForecastService();
    }

    [Test]
    public void ShouldReturnFrom1To5Entries()
    {
        var data = _service.Get();
        var length = data.Count();
        
        Assert.GreaterOrEqual(length, 1);
        Assert.LessOrEqual(length, 5);
    }
    
    [Test]
    public void ShouldHaveNormalTemperature()
    {
        var data = _service.Get();
        var minTemperature = -20;
        var maxTemperature = 55;

        foreach (var entity in data)
        {
            Assert.GreaterOrEqual(entity.TemperatureC, minTemperature);
            Assert.LessOrEqual(entity.TemperatureC, maxTemperature);
        }
    }
}