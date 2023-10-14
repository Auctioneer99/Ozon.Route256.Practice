namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.OrdersEvents.Models;

public sealed record OrdersEvent(long OrderId, OrderState OrderState, DateTime ChangedAt);