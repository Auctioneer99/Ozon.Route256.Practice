using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.OrdersEvents.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.OrdersEvents;

public sealed class OrdersEventsService
{
    private readonly IOrderRepository _orderRepository;

    public OrdersEventsService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<bool> Handle(OrdersEvent ordersEvent, CancellationToken token)
    {
        try
        {
            await _orderRepository.UpdateOrderStatus(ordersEvent.OrderId, ordersEvent.OrderState.ToDomain(), token);
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}