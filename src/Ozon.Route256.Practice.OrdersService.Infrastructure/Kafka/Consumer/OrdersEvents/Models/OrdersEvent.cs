namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.OrdersEvents.Models;

public sealed record OrdersEvent(
    long OrderId, 
    OrderState OrderState, 
    DateTime ChangedAt);