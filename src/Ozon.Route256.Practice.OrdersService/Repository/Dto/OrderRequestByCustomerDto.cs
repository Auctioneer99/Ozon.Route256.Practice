namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public sealed record OrderRequestByCustomerDto(
    long CustomerId,
    DateTime From,
    long SkipCount,
    long TakeCount
);