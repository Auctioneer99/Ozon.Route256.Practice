namespace Ozon.Route256.Practice.OrdersService.Domain.Models.Primitives;

public sealed class Point
{
    public decimal Longitude
    {
        get => _longitude;
        set 
        {
            if (value < -180 || value > 180)
            {
                throw new ArgumentException("Longitude");
            }

            _longitude = value;
        }
    }
    private decimal _longitude;

    public decimal Latitude
    {
        get => _latitude;
        set
        {
            if (value < -90 || value > 90)
            {
                throw new ArgumentException("Latitude");
            }

            _latitude = value;
        }
    }
    private decimal _latitude;

    public Point() : this(0, 0)
    {
    }

    public Point(decimal longitude, decimal latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }
}