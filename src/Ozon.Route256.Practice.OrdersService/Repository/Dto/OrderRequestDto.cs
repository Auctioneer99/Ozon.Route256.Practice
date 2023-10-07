namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public sealed record OrderRequestDto(
    SortingType SortingType,
    OrderField OrderField,
    long TakeCount,
    long SkipCount,
    OrderType OrderType,
    IEnumerable<long> Regions
);