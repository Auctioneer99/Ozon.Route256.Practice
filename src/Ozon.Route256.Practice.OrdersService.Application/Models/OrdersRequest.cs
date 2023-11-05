using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Models;

public sealed record OrdersRequest(
    SortingType SortingType, 
    OrderField OrderField, 
    OrderType OrderType, 
    long TakeCount,
    long SkipCount, 
    IEnumerable<string> Regions, 
    DateTime From);