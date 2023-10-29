using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Services.Mapping;

public static class KafkaMappingExtensions
{
    public static OrderState ToDto(this Kafka.Consumer.OrdersEvents.Models.OrderState state)
    {
        return (OrderState)state;
    }
    
    public static OrderType ToDto(this Kafka.Consumer.PreOrders.Models.OrderSource source)
    {
        return (OrderType)source;
    }
    
    public static OrderDto ToDto(this Kafka.Consumer.PreOrders.Models.PreOrder order, long regionFromId, DateTime createdAt)
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
            createdAt);
    }

    public static AddressDto ToDto(this Kafka.Consumer.PreOrders.Models.PreAddress address, long regionId, long orderId, long customerId)
    {
        return new AddressDto(
            0,
            regionId,
            orderId,
            customerId,
            address.City,
            address.Street,
            address.Building,
            address.Apartment,
            address.Latitude,
            address.Longitude);
    }
}