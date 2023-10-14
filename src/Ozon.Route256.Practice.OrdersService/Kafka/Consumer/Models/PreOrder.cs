namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.Models;

public sealed record PreOrder(
    long Id,
    OrderSource Source,
    PreCustomer Customer,
    PreGood[] Goods);