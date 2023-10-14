using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public interface IOrderRepository
{
    public Task<OrderDto> GetById(long orderId, CancellationToken token);
    
    public Task<OrderDto?> FindOrder(long orderId, CancellationToken token);
    
    public Task<OrderDto[]> GetAll(CancellationToken token);
    
    public Task<OrderDto[]> GetAll(OrderRequestDto orderRequest, CancellationToken token);

    public Task<OrderDto[]> GetByCustomerId(OrderRequestByCustomerDto orderRequest, CancellationToken token);

    public Task Add(OrderDto order, CancellationToken token);
    
    public Task UpdateOrderStatus(long orderId, OrderState state, CancellationToken token);
}