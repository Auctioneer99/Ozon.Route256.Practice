namespace Ozon.Route256.Practice.OrdersService.Application.Models;

public sealed record CustomerOrdersRequest(
    long CustomerId,
    DateTime From,
    long SkipCount,
    long TakeCount);