using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Repository.Models;

public sealed record OrderRepositoryRequest(
    SortingType SortingType,
    OrderField OrderField,
    long TakeCount,
    long SkipCount,
    OrderType OrderType,
    IEnumerable<long> Regions,
    DateTime From);