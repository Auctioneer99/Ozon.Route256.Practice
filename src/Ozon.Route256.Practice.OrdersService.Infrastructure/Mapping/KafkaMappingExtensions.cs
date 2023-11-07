using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;

internal static class KafkaMappingExtensions
{
    public static OrderState ToDomain(this Infrastructure.Kafka.Consumer.OrdersEvents.Models.OrderState state)
    {
        return state switch
        {
            Kafka.Consumer.OrdersEvents.Models.OrderState.Created => OrderState.Created,
            Kafka.Consumer.OrdersEvents.Models.OrderState.SentToCustomer => OrderState.SentToCustomer,
            Kafka.Consumer.OrdersEvents.Models.OrderState.Delivered => OrderState.Delivered,
            Kafka.Consumer.OrdersEvents.Models.OrderState.Lost => OrderState.Lost,
            Kafka.Consumer.OrdersEvents.Models.OrderState.Cancelled => OrderState.Cancelled,
            
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
    
    public static OrderType ToDomain(this OrderSource source)
    {
        return source switch
        {
            OrderSource.Web => OrderType.Web,
            OrderSource.Mobile => OrderType.Mobile,
            OrderSource.Api => OrderType.Api,
            
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
    
    public static Order ToDomain(this PreOrder order, long regionFromId, DateTime createdAt)
    {
        return new Order(
            order.Id,
            order.Goods.Sum(g => g.Quantity),
            order.Goods.Sum(g => (decimal)g.Price),
            order.Goods.Sum(g => (decimal)g.Weight),
            order.Source.ToDomain(),
            OrderState.Created,
            regionFromId,
            order.Customer.Id,
            createdAt);
    }

    public static Address ToDomain(this PreAddress address, long regionId, long orderId, long customerId)
    {
        return new Address(
            orderId,
            regionId,
            orderId,
            customerId,
            address.City,
            address.Street,
            address.Building,
            address.Apartment,
            (decimal)address.Latitude,
            (decimal)address.Longitude);
    }
}