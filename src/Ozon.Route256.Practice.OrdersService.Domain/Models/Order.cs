using Ozon.Route256.Practice.OrdersService.Exceptions;

namespace Ozon.Route256.Practice.OrdersService.Domain.Models;

public sealed class Order
{
    public long Id { get; }
    
    public long Count { get; private set; }
    
    public decimal TotalSum { get; private set; }
    
    public decimal TotalWeight { get; private set; }
    
    public OrderType Type { get; private set; }
    
    public OrderState State { get; private set; }
    
    public long RegionFromId { get; private set; }
    
    public long CustomerId { get; }
    
    public DateTime CreatedAt { get; }

    public Order(long id, long count, decimal totalSum, decimal totalWeight, OrderType type, OrderState state, 
        long regionFromId, long customerId, DateTime createdAt)
    {
        Id = id;
        Count = count;
        TotalSum = totalSum;
        TotalWeight = totalWeight;
        Type = type;
        State = state;
        RegionFromId = regionFromId;
        CustomerId = customerId;
        CreatedAt = createdAt;
    }

    public void UpdateState(OrderState state)
    {
        if (State == OrderState.Delivered || State == OrderState.Cancelled)
        {
            throw new UnavailableStateChangeException($"Not valid state change {State} -> {state}");
        }

        State = state;
    }
}