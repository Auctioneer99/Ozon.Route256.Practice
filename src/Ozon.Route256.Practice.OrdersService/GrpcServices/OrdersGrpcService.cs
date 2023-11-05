using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Application.Services;
using Ozon.Route256.Practice.OrdersService.Grpc.Orders;
using Ozon.Route256.Practice.OrdersService.Mapping;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices;

internal sealed class OrdersGrpcService : Orders.OrdersBase
{
    private readonly IOrderService _orderService;
    private readonly IRegionService _regionService;

    public OrdersGrpcService(IOrderService orderService, IRegionService regionService)
    {
        _orderService = orderService;
        _regionService = regionService;
    }

    public override async Task<CancelResponse> CancelOrder(CancelRequest request, ServerCallContext context)
    {
        var response = await _orderService.CancelOrder(request.Id, context.CancellationToken);

        return response.ToHost();
    }

    public override async Task<GetStatusByIdResponse> GetStatusById(GetStatusByIdRequest request,
        ServerCallContext context)
    {
        var status = await _orderService.GetStatusById(request.Id, context.CancellationToken);
        
        return new GetStatusByIdResponse()
        {
            State = status.ToHost()
        };
    }

    public override async Task<GetRegionsResponse> GetRegions(Empty request, ServerCallContext context)
    {
        var regions = await _regionService.GetRegions(context.CancellationToken);

        return new GetRegionsResponse()
        {
            Regions = { regions.Select(r => r.Name) }
        };
    }

    public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
    {
        var applicationRequest = request.ToApplication();
        var response = await _orderService.GetOrders(applicationRequest, context.CancellationToken);

        return new GetOrdersResponse()
        {
            Orders = { response.Select(o => o.ToHost()) }
        };
    }

    public override async Task<GetOrdersAggregationResponse> GetOrdersAggregation(GetOrdersAggregationRequest request,
        ServerCallContext context)
    {
        var applicationRequest = request.ToApplication();
        var response = await _orderService.GetOrdersAggregation(applicationRequest, context.CancellationToken);

        return new GetOrdersAggregationResponse()
        {
            Aggregations = { response.Select(e => e.ToHost()) }
        };
    }

    public override async Task<GetCustomerOrdersResponse> GetCustomerOrders(GetCustomerOrdersRequest request,
        ServerCallContext context)
    {
        var applicationRequest = request.ToApplication();
        var response = await _orderService.GetCustomerOrders(applicationRequest, context.CancellationToken);

        return new GetCustomerOrdersResponse()
        {
            Orders = { response.Select(o => o.ToHost()) }
        };
    }
}