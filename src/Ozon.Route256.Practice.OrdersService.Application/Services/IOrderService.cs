using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Services;

public interface IOrderService
{
    public Task<OrderState> GetStatusById(long id, CancellationToken token);

    public Task<OrderAggregate[]> GetCustomerOrders(CustomerOrdersRequest request, CancellationToken token);

    public Task<OrderAggregate[]> GetOrders(OrdersRequest request, CancellationToken token);

    public Task<OrderAggregation[]> GetOrdersAggregation(OrdersAggregationRequest request, CancellationToken token);
    
    public Task<CancelOrderResponse> CancelOrder(long id, CancellationToken token);
}