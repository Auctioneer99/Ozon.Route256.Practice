using Ozon.Route256.Practice.OrdersService.Kafka.Consumer.PreOrders.Models;
using Ozon.Route256.Practice.OrdersService.Kafka.Producer;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Test;

public class KafkaValidatorTests
{
    private readonly NewOrderValidator _newOrderValidator = new();

    [Fact]
    public void ZeroDistanceTest()
    {
        var address = new PreAddress(
            "",
            "",
            "",
            "",
            "",
            0,
            0
            );
        var region = new RegionDto(
            0,
            "",
            0,
            0);
        
        var valid = _newOrderValidator.ValidateDistance(address, region);
        
        Assert.True(valid);
    }
    
    [Fact]
    public void SomeValidDistanceTest()
    {
        var address = new PreAddress(
            "",
            "",
            "",
            "",
            "",
            2,
            1
        );
        var region = new RegionDto(
            0,
            "",
            1,
            2);
        
        var valid = _newOrderValidator.ValidateDistance(address, region);
        
        Assert.True(valid);
    }
    
    [Fact]
    public void SomeFarDistanceTest()
    {
        var address = new PreAddress(
            "",
            "",
            "",
            "",
            "",
            100,
            100
        );
        var region = new RegionDto(
            0,
            "",
            0,
            0);
        
        var valid = _newOrderValidator.ValidateDistance(address, region);
        
        Assert.False(valid);
    }
}