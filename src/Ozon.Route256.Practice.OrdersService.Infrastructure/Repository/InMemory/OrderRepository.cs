using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Application.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.InMemory.Dal;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.InMemory;

public sealed class OrderRepository : IOrderRepository
{
    private readonly InMemoryStorage _storage;

    public OrderRepository(InMemoryStorage storage)
    {
        _storage = storage;
    }

    public Task<Order> GetById(long orderId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Order>(token);
        }
        
        return _storage.Orders.TryGetValue(orderId, out var order)
            ? Task.FromResult(order)
            : throw new NotFoundException($"Заказ с Id = {orderId} не найден");
    }
    
    public Task<Order?> FindById(long orderId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Order?>(token);
        }

        return _storage.Orders.TryGetValue(orderId, out var order)
            ? Task.FromResult<Order?>(order)
            : Task.FromResult<Order?>(null);
    }

    public Task<Order[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Order[]>(token);
        }

        return Task.FromResult(_storage.Orders.Values.ToArray());
    }

    public Task<Order[]> GetByCustomerId(long customerId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Order[]>(token);
        }

        return Task.FromResult(FindDto(o => o.CustomerId == customerId, token).ToArray());
    }

    public Task<Order[]> GetAll(OrderRepositoryRequest orderRequest, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Order[]>(token);
        }

        var data = _storage.Orders.Values.AsEnumerable();
        
        data = data.Where(o => orderRequest.Regions.Contains(o.RegionFromId));

        data = data
            .Where(o => o.Type == orderRequest.OrderType);

        Func<Order, object>? selector = null;
        switch (orderRequest.OrderField)
        {
            case OrderField.NoneField:
                break;
            case OrderField.Id:
                selector = o => o.Id;
                break;
            case OrderField.Count:
                selector = o => o.Count;
                break;
            case OrderField.TotalSum:
                selector = o => o.TotalSum;
                break;
            case OrderField.TotalWeight:
                selector = o => o.TotalWeight;
                break;
            case OrderField.OrderType:
                selector = o => o.Type;
                break;
            case OrderField.CreatedAt:
                selector = o => o.CreatedAt;
                break;
            case OrderField.OrderState:
                selector = o => o.State;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(OrderField));
        }
        
        if (selector is not null)
        {
            switch (orderRequest.SortingType)
            {
                case SortingType.None:
                    break;
                case SortingType.Ascending:
                    data = data.OrderBy(selector);
                    break;
                case SortingType.Descending:
                    data = data.OrderByDescending(selector);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(SortingType));
            }
        }

        data = data
            .Skip((int)orderRequest.SkipCount);
        
        if (orderRequest.TakeCount > 0)
        {
            data = data.Take((int)orderRequest.TakeCount);
        }

        return Task.FromResult(data.ToArray());
    }

    public Task<Order[]> GetByCustomerId(OrderByCustomerRepositoryRequest orderRequest, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Order[]>(token);
        }

        var data = _storage.Orders.Values.AsEnumerable();

        data = data
            .Where(o => o.CustomerId == orderRequest.CustomerId)
            .Where(o => o.CreatedAt >= orderRequest.From);

        data = data
            .Skip((int)orderRequest.SkipCount);
        
        if (orderRequest.TakeCount > 0)
        {
            data = data.Take((int)orderRequest.TakeCount);
        }

        return Task.FromResult(data.ToArray());
    }

    public Task Add(Order order, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled(token);
        }

        _storage.Orders[order.Id] = order;
        
        return Task.CompletedTask;
    }

    public async Task UpdateOrderStatus(long orderId, OrderState state, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var order = await GetById(orderId, token);
        
        order.UpdateState(state);

        _storage.Orders[order.Id] = order;
    }
    
    private IEnumerable<Order> FindDto(Predicate<Order> predicate, CancellationToken token)
    {
        foreach (var value in _storage.Orders.Values)
        {
            token.ThrowIfCancellationRequested();

            if (predicate(value))
            {
                yield return value;
            }
        }
    }
}