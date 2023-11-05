using Ozon.Route256.Practice.OrdersService.Domain.Models.Primitives;

namespace Ozon.Route256.Practice.OrdersService.Domain.Models;

public sealed class Region
{
    public long Id { get; private set; }
    
    public string Name { get; private set; }

    public decimal Latitude => Point.Latitude;

    public decimal Longitude => Point.Longitude;
    
    public Point Point { get; private set; }

    public Region(long id, string name, decimal latitude, decimal longitude)
    {
        Id = id;
        Name = name;
        Point = new Point(longitude, latitude);
    }
}