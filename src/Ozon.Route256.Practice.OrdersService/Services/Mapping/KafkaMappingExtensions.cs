using Ozon.Route256.Practice.OrdersService.Kafka.Consumer.Models;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Services.Mapping;

public static class KafkaMappingExtensions
{
    public static OrderType ToDto(this OrderSource source)
    {
        return (OrderType)source;
    }
    
    public static OrderDto ToDto(this PreOrder order, long regionFromId, long addressId, DateTime createdAt)
    {
        return new OrderDto(
            order.Id,
            order.Goods.Sum(g => g.Quantity),
            order.Goods.Sum(g => g.Price),
            order.Goods.Sum(g => g.Weight),
            order.Source.ToDto(),
            OrderState.Created,
            regionFromId,
            order.Customer.Id,
            addressId,
            createdAt);
    }

    public static AddressDto ToDto(this PreAddress address, long regionId)
    {
        return new AddressDto(
            0,
            regionId,
            address.City,
            address.Street,
            address.Building,
            address.Apartment,
            address.Latitude,
            address.Longitude);
    }
}