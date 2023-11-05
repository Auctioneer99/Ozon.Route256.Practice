namespace Ozon.Route256.Practice.OrdersService.Application.Models;

public sealed record OrderAggregation(
    string Region,
    long UniqueCustomersCount,
    long OrdersCount,
    decimal TotalOrdersSum,
    decimal TotalOrdersWeight);