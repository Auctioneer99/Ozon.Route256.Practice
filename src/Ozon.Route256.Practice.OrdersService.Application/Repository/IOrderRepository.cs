using Ozon.Route256.Practice.OrdersService.Application.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Repository;

public interface IOrderRepository
{
    public Task<Order?> FindById(long orderId, CancellationToken token);
    
    public Task<Order> GetById(long orderId, CancellationToken token);

    public Task<Order[]> GetByCustomerId(OrderByCustomerRepositoryRequest orderByCustomerRepositoryRequest, CancellationToken token);
    
    public Task<Order[]> GetAll(CancellationToken token);
    
    public Task<Order[]> GetAll(OrderRepositoryRequest orderRepositoryRequest, CancellationToken token);

    public Task Add(Order order, CancellationToken token);
    
    public Task UpdateOrderStatus(long orderId, OrderState state, CancellationToken token);
}