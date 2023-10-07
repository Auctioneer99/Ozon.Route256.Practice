namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public sealed record OrderDto(
    long Id,
    int Count,
    double TotalSum,
    double TotalWeight,
    OrderType Type,
    OrderState State,
    long RegionFromId,
    long CustomerId,
    long AddressId,
    DateTime CreatedAt
);