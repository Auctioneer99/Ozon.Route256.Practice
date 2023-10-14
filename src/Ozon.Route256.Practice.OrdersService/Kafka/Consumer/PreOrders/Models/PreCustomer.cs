namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.PreOrders.Models;

public sealed record PreCustomer(
    long Id,
    PreAddress Address);