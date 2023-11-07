using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Application.Repository.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Mapping;

internal static class OrderMappingExtensions
{
    public static OrderRepositoryRequest ToRepository(this OrdersRequest request, IEnumerable<long> regionIds)
    {
        return new OrderRepositoryRequest(
            SortingType: request.SortingType, 
            OrderField: request.OrderField,
            TakeCount: request.TakeCount, 
            SkipCount: request.SkipCount, 
            OrderType: request.OrderType, 
            Regions: regionIds,
            From: request.From);
    }

    public static OrderByCustomerRepositoryRequest ToRepository(this CustomerOrdersRequest request)
    {
        return new OrderByCustomerRepositoryRequest(
            CustomerId: request.CustomerId, 
            From: request.From,
            SkipCount: request.SkipCount, 
            TakeCount: request.TakeCount);
    }
}