namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.Models;

public sealed record PreCustomer(
    long Id,
    PreAddress Address);