namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders.Models;

public sealed record PreOrder(
    long Id,
    OrderSource Source,
    PreCustomer Customer,
    PreGood[] Goods);