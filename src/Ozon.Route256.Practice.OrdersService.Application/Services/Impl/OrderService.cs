using Microsoft.Extensions.Logging;
using Ozon.Route256.Practice.OrdersService.Application.Mapping;
using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Application.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Services.Impl;

public sealed class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    
    private readonly IRegionRepository _regionRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ILogisticsRepository _logisticsRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrderService(
        ILogger<OrderService> logger,
        IRegionRepository regionRepository, 
        IOrderRepository orderRepository,
        IAddressRepository addressRepository,
        ILogisticsRepository logisticsRepository, 
        ICustomerRepository customerRepository)
    {
        _logger = logger;
        _regionRepository = regionRepository;
        _orderRepository = orderRepository;
        _addressRepository = addressRepository;
        _logisticsRepository = logisticsRepository;
        _customerRepository = customerRepository;
    }

    public async Task<OrderState> GetStatusById(long id, CancellationToken token)
    {
        _logger.LogDebug("Get status request {@Id}", id);
        var order = await _orderRepository.GetById(id, token);
        _logger.LogDebug("Get status response {@State}", order.State);
        return order.State;
    }

    public async Task<OrderAggregate[]> GetCustomerOrders(CustomerOrdersRequest request,
        CancellationToken token)
    {
        _logger.LogDebug("Get customer orders request {@Request}", request);
        
        var repositoryRequest = request.ToRepository();
        var customer = await _customerRepository.GetById(request.CustomerId, token);
        var orders = await _orderRepository.GetByCustomerId(repositoryRequest, token);
        var addresses = await _addressRepository.GetManyByOrderId(orders.Select(o => o.Id), token);
        var regions = await _regionRepository.GetManyById(addresses.Select(a => a.RegionId), token);

        var response = orders
            .Select(o => (order: o, address: addresses.First(a => a.OrderId == o.Id)))
            .Select(pair => new OrderAggregate(
                address: pair.address,
                customer: customer,
                order: pair.order,
                region: regions.First(r => r.Id == pair.address.RegionId)));

        _logger.LogDebug("Get customer orders response {@Response}", response);

        return response.ToArray();
    }

    public async Task<OrderAggregate[]> GetOrders(OrdersRequest request, CancellationToken token)
    {
        _logger.LogDebug("Get orders request {@Request}", request);
        
        var regions = await _regionRepository.GetManyByName(request.Regions, token);

        var orderRequest = request.ToRepository(regions.Select(r => r.Id));
        var orders = await _orderRepository.GetAll(orderRequest, token);
        var addresses = await _addressRepository.GetManyByOrderId(orders.Select(o => o.Id), token);
        var regionAddresses = await _regionRepository.GetManyById(addresses.Select(a => a.RegionId).Distinct(), token);
        var customers = await _customerRepository.GetManyById(orders.Select(o => o.CustomerId).Distinct(), token);

        var response = orders
            .Select(o => (order: o, address: addresses.First(a => a.OrderId == o.Id)))
            .Select(pair => new OrderAggregate(
                address: pair.address,
                customer: customers.First(c => c.Id == pair.order.CustomerId),
                order: pair.order,
                region: regionAddresses.First(r => r.Id == pair.address.RegionId)));
        
        _logger.LogDebug("Get orders response {@Response}", response);
        
        return response.ToArray();
    }

    public async Task<OrderAggregation[]> GetOrdersAggregation(OrdersAggregationRequest request,
        CancellationToken token)
    {
        _logger.LogDebug("Get order aggregation request {@Request}", request);
        
        var regions = await _regionRepository.GetManyByName(request.Regions, token);

        var orderRequest = new OrderRepositoryRequest(
            SortingType: SortingType.None,
            OrderField: OrderField.NoneField,
            TakeCount: 0,
            SkipCount: 0,
            OrderType: OrderType.Undefined,
            Regions: regions.Select(r => r.Id),
            From: request.FromDate
        );
        var orders = await _orderRepository.GetAll(orderRequest, token);

        var response = orders
            .GroupBy(o => o.RegionFromId)
            .Select(group => new OrderAggregation(
                Region: regions.First(r => r.Id == group.Key).Name,
                OrdersCount: group.Count(),
                TotalOrdersSum: group.Sum(o => o.TotalSum),
                TotalOrdersWeight: group.Sum(o => o.TotalWeight),
                UniqueCustomersCount: group.Select(o => o.CustomerId).Distinct().Count()));
        
        _logger.LogDebug("Get order aggregation response {@Response}", response);
        
        return response.ToArray();
    }

    public async Task<CancelOrderResponse> CancelOrder(long id, CancellationToken token)
    {
        _logger.LogDebug("Cancel order request {@Id}", id);
        
        var order = await _orderRepository.GetById(id, token);

        if (order.State == OrderState.Delivered)
        {
            var result = new CancelOrderResponse(
                Success: false,
                Error: "Заказ на последней стадии оформления");
            
            _logger.LogDebug("Cancel order response {@Response}", result);

            return result;
        }

        var response = await _logisticsRepository.CancelOrder(id, token);

        if (response.Success == true)
        {
            await _orderRepository.UpdateOrderStatus(id, OrderState.Cancelled, token);
        }

        _logger.LogDebug("Cancel order response {@Response}", response);
        
        return response;
    }
}