﻿using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;

public class NewOrderValidator
{
    public bool ValidateDistance(PreAddress address, Region region)
    {
        var radiusOfEarthInKilometers = 6371.0;
        var lat1Rad = Math.PI * (double)region.Latitude / 180;
        var lat2Rad = Math.PI * address.Latitude / 180;
        var deltaLat = Math.PI * (address.Latitude - (double)region.Latitude) / 180;
        var deltaLon = Math.PI * (address.Longitude - (double)region.Longitude) / 180;
        var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Cos(lat1Rad) * Math.Cos(lat2Rad) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        var distance = radiusOfEarthInKilometers * c;

        return distance < 5000;
    }
}