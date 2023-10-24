using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;
using Ozon.Route256.Practice.OrdersService.Services.Mapping;

namespace Ozon.Route256.Practice.OrdersService.GrpcServices;

public sealed class OrdersGrpcService : Grpc.Orders.Orders.OrdersBase
{
    private readonly IRegionRepository _regionRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ILogisticsRepository _logisticsRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrdersGrpcService(IRegionRepository regionRepository, 
        IOrderRepository orderRepository,
        IAddressRepository addressRepository,
        ILogisticsRepository logisticsRepository, 
        ICustomerRepository customerRepository)
    {
        _regionRepository = regionRepository;
        _orderRepository = orderRepository;
        _addressRepository = addressRepository;
        _logisticsRepository = logisticsRepository;
        _customerRepository = customerRepository;
    }

    public override async Task<Grpc.Orders.CancelResponse> CancelOrder(Grpc.Orders.CancelRequest request, ServerCallContext context)
    {
        var order = await _orderRepository.GetById(request.Id, context.CancellationToken);

        if (order.State == OrderState.Delivered)
        {
            return new Grpc.Orders.CancelResponse
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

        return new Grpc.Orders.CancelResponse
        {
            IsSuccess = true,
            Error = ""
        };
    }

    public override async Task<Grpc.Orders.GetStatusByIdResponse> GetStatusById(Grpc.Orders.GetStatusByIdRequest request,
        ServerCallContext context)
    {
        var order = await _orderRepository.GetById(request.Id, context.CancellationToken);

        return new Grpc.Orders.GetStatusByIdResponse()
        {
            State = order.State.FromDto()
        };
    }

    public override async Task<Grpc.Orders.GetRegionsResponse> GetRegions(Grpc.Orders.Empty request, ServerCallContext context)
    {
        var regions = await _regionRepository.GetAll(context.CancellationToken);

        return new Grpc.Orders.GetRegionsResponse()
        {
            Regions = { regions.Select(r => r.Name) }
        };
    }

    public override async Task<Grpc.Orders.GetOrdersResponse> GetOrders(Grpc.Orders.GetOrdersRequest request, ServerCallContext context)
    {
        var regions = await _regionRepository.GetManyByName(request.RegionFilter, context.CancellationToken);

        var orderRequest = request.ToDto(regions.Select(r => r.Id));
        var orders = await _orderRepository.GetAll(orderRequest, context.CancellationToken);

        var addresses =
            await _addressRepository.GetManyById(orders.Select(o => 0l), context.CancellationToken);
        var regionAddresses =
            await _regionRepository.GetManyById(addresses.Select(a => a.RegionId).Distinct(),
                context.CancellationToken);
        var customers =
            await _customerRepository.GetManyById(orders.Select(o => o.CustomerId).Distinct(),
                context.CancellationToken);

        return new Grpc.Orders.GetOrdersResponse()
        {
            Orders =
            {
                orders.Select(o => new { Order = o, Address = addresses.First(a => a.Id == 0) })
                    .Select(pair => pair.Order.FromDto(
                        regions.First(r => r.Id == pair.Order.RegionFromId),
                        customers.First(c => c.Id == pair.Order.CustomerId),
                        pair.Address.FromDto(regionAddresses.First(ra => ra.Id == pair.Address.RegionId)
                        )))
            }
        };
    }

    public override async Task<Grpc.Orders.GetOrdersAggregationResponse> GetOrdersAggregation(Grpc.Orders.GetOrdersAggregationRequest request,
        ServerCallContext context)
    {
        var regions = await _regionRepository.GetManyByName(request.Regions, context.CancellationToken);

        var orderRequest = new OrderRequestDto(
            SortingType.None,
            OrderField.NoneField,
            0,
            0,
            OrderType.Undefined,
            regions.Select(r => r.Id)
        );
        var orders = await _orderRepository.GetAll(orderRequest, context.CancellationToken);

        var entries = new List<Grpc.Orders.GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry>(regions.Length);
        foreach (var group in orders.GroupBy(o => o.RegionFromId))
        {
            var entry = new Grpc.Orders.GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry
            {
                Region = regions.First(r => r.Id == group.Key).Name,
                OrdersCount = group.Count(),
                TotalOrdersSum = group.Sum(o => o.TotalSum),
                TotalOrdersWeight = group.Sum(o => o.TotalWeight),
                UniqueCustomersCount = group.Select(o => o.CustomerId).Distinct().Count()
            };
            entries.Add(entry);
        }

        return new Grpc.Orders.GetOrdersAggregationResponse()
        {
            Aggregations = { entries }
        };
    }

    public override async Task<Grpc.Orders.GetCustomerOrdersResponse> GetCustomerOrders(Grpc.Orders.GetCustomerOrdersRequest request,
        ServerCallContext context)
    {
        var orderRequest = request.ToDto();
        var orders = await _orderRepository.GetByCustomerId(orderRequest, context.CancellationToken);
        var addresses =
            await _addressRepository.GetManyById(orders.Select(o => 0l), context.CancellationToken);
        var regions = await _regionRepository.GetManyById(
            addresses.Select(a => a.RegionId).Union(orders.Select(o => o.RegionFromId)).Distinct(),
            context.CancellationToken);
        var customer = await _customerRepository.GetById(request.CustomerId, context.CancellationToken);

        return new Grpc.Orders.GetCustomerOrdersResponse()
        {
            Orders =
            {
                orders.Select(o => new { Order = o, Address = addresses.First(a => a.Id == 0l) })
                    .Select(pair => pair.Order.FromDto(
                        regions.First(r => r.Id == pair.Order.RegionFromId),
                        customer,
                        pair.Address.FromDto(regions.First(r => r.Id == pair.Address.RegionId)
                        )))
            }
        };
    }
}