namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public sealed record OrderDto(
    long Id,
    int Count,
    decimal TotalSum,
    decimal TotalWeight,
    int Type,
    int State,
    long RegionFromId,
    long CustomerId,
    DateTime CreatedAt);