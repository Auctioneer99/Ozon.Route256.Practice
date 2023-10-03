using Ozon.Route256.Practice.OrdersService.Services;

namespace Ozon.Route256.Practice.OrdersService.Test;

public sealed class RegionsTest
{
    private readonly RegionService _regionService;
    
    public RegionsTest()
    {
        _regionService = new RegionService();
    }
    
    [Fact]
    public void ShouldFindRuRegion()
    {
        Assert.True(_regionService.HasRegion("RU"));
    }
}