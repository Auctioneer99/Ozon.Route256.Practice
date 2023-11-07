namespace Ozon.Route256.Practice.OrdersService.Application.Repository.Models;

public sealed record OrderByCustomerRepositoryRequest(
    long CustomerId,
    DateTime From,
    long SkipCount,
    long TakeCount);