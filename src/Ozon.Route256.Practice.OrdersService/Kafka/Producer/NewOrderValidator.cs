using Ozon.Route256.Practice.OrdersService.Kafka.Consumer.PreOrders.Models;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Producer;

public class NewOrderValidator
{
    public bool ValidateDistance(PreAddress address, RegionDto region)
    {
        var radiusOfEarthInKilometers = 6371.0;
        var lat1Rad = Math.PI * region.Latitude / 180;
        var lat2Rad = Math.PI * address.Latitude / 180;
        var deltaLat = Math.PI * (address.Latitude - region.Latitude) / 180;
        var deltaLon = Math.PI * (address.Longitude - region.Longitude) / 180;
        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = radiusOfEarthInKilometers * c;

        return distance < 5000;
    }
}