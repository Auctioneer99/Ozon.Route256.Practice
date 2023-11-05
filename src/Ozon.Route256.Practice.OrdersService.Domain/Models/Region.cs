namespace Ozon.Route256.Practice.OrdersService.Domain.Models;

public sealed class Region
{
    public long Id { get; private set; }
    
    public string Name { get; private set; }
    
    public decimal Latitude { get; private set; }
    
    public decimal Longitude { get; private set; }

    public Region(long id, string name, decimal latitude, decimal longitude)
    {
        Id = id;
        Name = name;
        Latitude = latitude;
        Longitude = longitude;
    }
}