using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.InMemory;

public sealed class OrderRepository : IOrderRepository
{
    private readonly InMemoryStorage _storage;

    public OrderRepository(InMemoryStorage storage)
    {
        _storage = storage;
    }

    public Task<OrderDto> GetById(long orderId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto>(token);
        }
        
        return _storage.Orders.TryGetValue(orderId, out var order)
            ? Task.FromResult(order)
            : throw new NotFoundException($"Заказ с Id = {orderId} не найден");
    }
    
    public Task<OrderDto?> FindById(long orderId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto?>(token);
        }

        return _storage.Orders.TryGetValue(orderId, out var order)
            ? Task.FromResult<OrderDto?>(order)
            : Task.FromResult<OrderDto?>(null);
    }

    public Task<OrderDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
        }

        return Task.FromResult(_storage.Orders.Values.ToArray());
    }

    public Task<OrderDto[]> GetByCustomerId(long customerId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
        }

        return Task.FromResult(FindDto(o => o.CustomerId == customerId, token).ToArray());
    }

    public Task<OrderDto[]> GetAll(OrderRequestDto orderRequest, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
        }

        var data = _storage.Orders.Values.AsEnumerable();
        
        data = data.Where(o => orderRequest.Regions.Contains(o.RegionFromId));

        data = data
            .Where(o => o.Type == (int)orderRequest.OrderType);

        Func<OrderDto, object>? selector = null;
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

    public Task<OrderDto[]> GetByCustomerId(OrderRequestByCustomerDto orderRequest, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
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

    public Task Add(OrderDto order, CancellationToken token)
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

        _storage.Orders[order.Id] = order with { State = (int)state };
    }
    
    private IEnumerable<OrderDto> FindDto(Predicate<OrderDto> predicate, CancellationToken token)
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