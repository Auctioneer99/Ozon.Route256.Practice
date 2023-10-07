using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;
using Ozon.Route256.Practice.OrdersService.Services.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Controllers;

public sealed class OrdersGrpcService : Orders.OrdersBase
{
    private readonly IRegionRepository _regionRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ILogisticsRepository _logisticsRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrdersGrpcService(IRegionRepository regionRepository, IOrderRepository orderRepository,
        IAddressRepository addressRepository,
        ILogisticsRepository logisticsRepository, ICustomerRepository customerRepository)
    {
        _regionRepository = regionRepository;
        _orderRepository = orderRepository;
        _addressRepository = addressRepository;
        _logisticsRepository = logisticsRepository;
        _customerRepository = customerRepository;
    }

    public override async Task<CancelResponse> CancelOrder(CancelRequest request, ServerCallContext context)
    {
        var order = await _orderRepository.GetById(request.Id, context.CancellationToken);

        if (order.State == OrderState.Delivered)
        {
            return new CancelResponse
            {
                IsSuccess = false,
                Error = "Заказ на последней стадии оформления"
            };
        }

        var response = await _logisticsRepository.CancelOrder(request.Id, context.CancellationToken);

        if (response.Success == false)
        {
            return response.FromDto();
        }

        await _orderRepository.UpdateOrderStatus(request.Id, OrderState.Cancelled, context.CancellationToken);

        return new CancelResponse
        {
            IsSuccess = true,
            Error = ""
        };
    }

    public override async Task<GetStatusByIdResponse> GetStatusById(GetStatusByIdRequest request,
        ServerCallContext context)
    {
        var order = await _orderRepository.GetById(request.Id, context.CancellationToken);

        return new GetStatusByIdResponse()
        {
            State = order.State.FromDto()
        };
    }

    public override async Task<GetRegionsResponse> GetRegions(Empty request, ServerCallContext context)
    {
        var regions = await _regionRepository.GetAll(context.CancellationToken);

        return new GetRegionsResponse()
        {
            Regions = { regions.Select(r => r.Name) }
        };
    }

    public override async Task<GetOrdersResponse> GetOrders(GetOrdersRequest request, ServerCallContext context)
    {
        var regions = await _regionRepository.GetManyByName(request.RegionFilter, context.CancellationToken);

        var orderRequest = request.ToDto(regions.Select(r => r.Id));
        var orders = await _orderRepository.GetAll(orderRequest, context.CancellationToken);

        var addresses =
            await _addressRepository.GetManyById(orders.Select(o => o.AddressId), context.CancellationToken);
        var regionAddresses =
            await _regionRepository.GetManyById(addresses.Select(a => a.RegionId).Distinct(),
                context.CancellationToken);
        var customers =
            await _customerRepository.GetManyById(orders.Select(o => o.CustomerId).Distinct(),
                context.CancellationToken);

        return new GetOrdersResponse()
        {
            Orders =
            {
                orders.Select(o => new { Order = o, Address = addresses.First(a => a.Id == o.AddressId) })
                    .Select(pair => pair.Order.FromDto(
                        regions.First(r => r.Id == pair.Order.RegionFromId),
                        customers.First(c => c.Id == pair.Order.CustomerId),
                        pair.Address.FromDto(regionAddresses.First(ra => ra.Id == pair.Address.RegionId)
                        )))
            }
        };
    }

    public override async Task<GetOrdersAggregationResponse> GetOrdersAggregation(GetOrdersAggregationRequest request,
        ServerCallContext context)
    {
        var regions = await _regionRepository.GetManyByName(request.Regions, context.CancellationToken);

        var orderRequest = new OrderRequestDto(
            SortingType.None,
            OrderField.NoneField,
            0,
            0,
            OrderType.UndefinedType,
            regions.Select(r => r.Id)
        );
        var orders = await _orderRepository.GetAll(orderRequest, context.CancellationToken);

        var entries = new List<GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry>(regions.Length);
        foreach (var group in orders.GroupBy(o => o.RegionFromId))
        {
            var entry = new GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry
            {
                Region = regions.First(r => r.Id == group.Key).Name,
                OrdersCount = group.Count(),
                TotalOrdersSum = group.Sum(o => o.TotalSum),
                TotalOrdersWeight = group.Sum(o => o.TotalWeight),
                UniqueCustomersCount = group.Select(o => o.CustomerId).Distinct().Count()
            };
            entries.Add(entry);
        }

        return new GetOrdersAggregationResponse()
        {
            Aggregations = { entries }
        };
    }

    public override async Task<GetCustomerOrdersResponse> GetCustomerOrders(GetCustomerOrdersRequest request,
        ServerCallContext context)
    {
        var orderRequest = request.ToDto();
        var orders = await _orderRepository.GetByCustomerId(orderRequest, context.CancellationToken);
        var addresses =
            await _addressRepository.GetManyById(orders.Select(o => o.AddressId), context.CancellationToken);
        var regions = await _regionRepository.GetManyById(
            addresses.Select(a => a.RegionId).Union(orders.Select(o => o.RegionFromId)).Distinct(),
            context.CancellationToken);
        var customers =
            await _customerRepository.GetManyById(orders.Select(o => o.CustomerId).Distinct(),
                context.CancellationToken);

        return new GetCustomerOrdersResponse()
        {
            Orders =
            {
                orders.Select(o => new { Order = o, Address = addresses.First(a => a.Id == o.AddressId) })
                    .Select(pair => pair.Order.FromDto(
                        regions.First(r => r.Id == pair.Order.RegionFromId),
                        customers.First(c => c.Id == pair.Order.CustomerId),
                        pair.Address.FromDto(regions.First(r => r.Id == pair.Address.RegionId)
                        )))
            }
        };
    }
}