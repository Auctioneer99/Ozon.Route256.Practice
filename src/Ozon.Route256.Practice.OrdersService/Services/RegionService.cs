namespace Ozon.Route256.Practice.OrdersService.Services;

public class RegionService
{
    public HashSet<string> GetRegions()
    {
        return new HashSet<string>()
        {
            "RU"
        };
    }

    public bool HasRegion(string region)
    {
        return GetRegions().Contains(region);
    }
}