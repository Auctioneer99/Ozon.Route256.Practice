using FakeItEasy;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Grpc.Orders;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;
using Empty = Ozon.Route256.Practice.OrdersService.Grpc.Orders.Empty;

namespace Ozon.Route256.Practice.OrdersService.Test;

public sealed class OrdersGrpcServiceTests
{
    private readonly OrdersGrpcService _service;
    private readonly ServerCallContext _context;

    public OrdersGrpcServiceTests()
    {
        var regionRepository = FakeRegionRepository();
        var orderRepository = FakeOrderRepository();
        var addressRepository = FakeAddressRepository();
        var logisticsRepository = FakeLogisticsRepository();
        var customerRepository = FakeCustomerRepository();

        _service = new OrdersGrpcService(
            regionRepository,
            orderRepository,
            addressRepository,
            logisticsRepository,
            customerRepository);
        _context = A.Fake<ServerCallContext>();
    }

    private IRegionRepository FakeRegionRepository()
    {
        var fake = A.Fake<IRegionRepository>();

        A.CallTo(() => fake.FindById(A<long>._, A<CancellationToken>._))
            .ReturnsLazily((long id, CancellationToken token) => CreateRegionDto(id));
        A.CallTo(() => fake.FindById(A<long>._, A<CancellationToken>._))
            .WhenArgumentsMatch((long id, CancellationToken token) => id <= 0 || id > 10)
            .ThrowsAsync((long id, CancellationToken token) => new NotFoundException(id.ToString()));
        
        A.CallTo(() => fake.GetManyById(A<IEnumerable<long>>._, A<CancellationToken>._))
            .ReturnsLazily((IEnumerable<long> ids, CancellationToken token) => 
                ids.Select(CreateRegionDto).ToArray());
        A.CallTo(() => fake.GetManyById(A<IEnumerable<long>>._, A<CancellationToken>._))
            .WhenArgumentsMatch((IEnumerable<long> ids, CancellationToken token) => ids.Any(v => v <= 0 || v > 10))
            .ThrowsAsync((IEnumerable<long> ids, CancellationToken token) => new NotFoundException(string.Join(", ", ids)));

        A.CallTo(() => fake.GetManyByName(A<IEnumerable<string>>._, A<CancellationToken>._))
            .ReturnsLazily((IEnumerable<string> names, CancellationToken token) =>
                names.Select(CreateRegionDto).ToArray());
        A.CallTo(() => fake.GetManyByName(A<IEnumerable<string>>._, A<CancellationToken>._))
            .WhenArgumentsMatch((IEnumerable<string> names, CancellationToken token) => names.Select(long.Parse).Any(v => v <= 0 || v > 10))
            .ThrowsAsync((IEnumerable<string> names, CancellationToken token) => new NotFoundException(string.Join(", ", names)));

        A.CallTo(() => fake.GetAll(A<CancellationToken>._))
            .ReturnsLazily((CancellationToken token) =>
                Enumerable.Range(1, 10).Select(id => CreateRegionDto(id)).ToArray());

        return fake;
    }

    private RegionDto CreateRegionDto(long id)
    {
        return new RegionDto(id, id.ToString(), id, id);
    }

    private RegionDto CreateRegionDto(string name)
    {
        var id = long.Parse(name);
        return new RegionDto(id, name, id, id);
    }
    
    private IOrderRepository FakeOrderRepository()
    {
        var fake = A.Fake<IOrderRepository>();

        A.CallTo(() => fake.GetById(A<long>._, A<CancellationToken>._))
            .ReturnsLazily((long id, CancellationToken token) => CreateOrderDto(id));
        A.CallTo(() => fake.GetById(A<long>._, A<CancellationToken>._))
            .WhenArgumentsMatch((long id, CancellationToken token) => id <= 0 || id > 100)
            .ThrowsAsync((long id, CancellationToken token) => new NotFoundException(id.ToString()));

        A.CallTo(() => fake.GetAll(A<OrderRequestDto>._, A<CancellationToken>._))
            .ReturnsLazily((OrderRequestDto request, CancellationToken token) => GetOrders(request));

        A.CallTo(() => fake.GetByCustomerId(A<OrderRequestByCustomerDto>._, A<CancellationToken>._))
            .ReturnsLazily((OrderRequestByCustomerDto request, CancellationToken token) => GetOrders(request));

        A.CallTo(() => fake.UpdateOrderStatus(A<long>._, A<OrderState>._, A<CancellationToken>._))
            .ReturnsLazily((long id, OrderState state, CancellationToken token) => Task.CompletedTask);
        A.CallTo(() => fake.UpdateOrderStatus(A<long>._, A<OrderState>._, A<CancellationToken>._))
            .WhenArgumentsMatch((long id, OrderState state, CancellationToken token) => id <= 0 || id > 100)
            .ThrowsAsync((long id, OrderState state, CancellationToken token) => new NotFoundException(id.ToString()));

        return fake;
    }

    private OrderDto CreateOrderDto(long id)
    {
        return new OrderDto(
            id,
            (int)(id * 2),
            id * 1.5,
            id * 3.5,
            (OrderType)(id % 2 + 1),
            (OrderState)(id % 5 + 1),
            id,
            id,
            id,
            DateTime.Now);
    }

    private OrderDto[] GetOrders(OrderRequestDto request)
    {
        return Enumerable.Range(1, 100)
            .Select(i => CreateOrderDto(i + request.SkipCount))
            .Where(i => i.Type == request.OrderType)
            .Where(i => request.Regions.Contains(i.RegionFromId))
            .Take((int)request.TakeCount)
            .ToArray();
    }

    private OrderDto[] GetOrders(OrderRequestByCustomerDto request)
    {
        return Enumerable.Range(1, 100)
            .Where(i => i <= 100)
            .Select(i => CreateOrderDto(i + request.SkipCount))
            .Where(i => i.CustomerId == request.CustomerId)
            .Take((int)request.TakeCount)
            .ToArray();
    }

    private IAddressRepository FakeAddressRepository()
    {
        var fake = A.Fake<IAddressRepository>();

        A.CallTo(() => fake.GetManyByOrderId(A<IEnumerable<long>>._, A<CancellationToken>._))
            .ReturnsLazily((IEnumerable<long> ids, CancellationToken token) => ids.Select(CreateAddressDto).ToArray());
        A.CallTo(() => fake.GetManyByOrderId(A<IEnumerable<long>>._, A<CancellationToken>._))
            .WhenArgumentsMatch((IEnumerable<long> ids, CancellationToken token) => ids.Any(id => id <= 0 || id > 100))
            .ThrowsAsync((IEnumerable<long> ids, CancellationToken token) =>
                throw new NotFoundException(string.Join(", ", ids)));
        
        return fake;
    }

    private AddressDto CreateAddressDto(long id)
    {
        return new AddressDto(
            id,
            id % 3 + 1,
            id.ToString() + "city",
            id.ToString() + "street",
            id.ToString() + "building",
            id.ToString() + "apartment",
            id / (double)long.MaxValue * 180,
            id / (double)long.MaxValue * 90);
    }

    private ILogisticsRepository FakeLogisticsRepository()
    {
        var fake = A.Fake<ILogisticsRepository>();

        A.CallTo(() => fake.CancelOrder(A<long>._, A<CancellationToken>._))
            .ReturnsLazily((long id, CancellationToken token) => CreateCancelResultDto(id));
        A.CallTo(() => fake.CancelOrder(A<long>._, A<CancellationToken>._))
            .WhenArgumentsMatch((long id, CancellationToken token) => id <= 0 || id > 100)
            .ThrowsAsync((long id, CancellationToken token) => new NotFoundException(id.ToString()));
        
        return fake;
    }

    private CancelResultDto CreateCancelResultDto(long id)
    {
        return new CancelResultDto(id % 2 == 1, id % 2 == 1 ? "" : "Error");
    }

    private ICustomerRepository FakeCustomerRepository()
    {
        var fake = A.Fake<ICustomerRepository>();

        A.CallTo(() => fake.GetById(A<long>._, A<CancellationToken>._))
            .ReturnsLazily((long id, CancellationToken token) => CreateCustomerDto(id));
        A.CallTo(() => fake.GetById(A<long>._, A<CancellationToken>._))
            .WhenArgumentsMatch((long id, CancellationToken token) => id <= 0 || id > 100)
            .ThrowsAsync((long id, CancellationToken token) => new NotFoundException(id.ToString()));

        A.CallTo(() =>
                fake.GetManyById(A<IEnumerable<long>>._, A<CancellationToken>._))
            .ReturnsLazily((IEnumerable<long> ids, CancellationToken token) => ids.Select(CreateCustomerDto).ToArray());
        A.CallTo(() => fake.GetManyById(A<IEnumerable<long>>._, A<CancellationToken>._))
            .WhenArgumentsMatch((IEnumerable<long> ids, CancellationToken token) => ids.Any(id => id <= 0 || id > 100))
            .ThrowsAsync((IEnumerable<long> ids, CancellationToken token) => throw new NotFoundException(string.Join(", ", ids)));
        
        return fake;
    }

    private CustomerDto CreateCustomerDto(long id)
    {
        return new CustomerDto((int)id, id.ToString() + "first", id.ToString() + "last", id.ToString(),
            id.ToString() + "@ru.ru");
    }
    
    [Fact]
    public async Task TestCancelFail()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.CancelOrder(new CancelRequest()
        {
            Id = 0
        }, _context));
        
        Assert.Equal("0", ex.Message);
    }
    
    [Fact]
    public async Task TestSuccessCancel()
    {
        var result = await _service.CancelOrder(new CancelRequest()
        {
            Id = 1
        }, _context);
        
        Assert.True(result.IsSuccess);
    }
    
    [Fact]
    public async Task TestLastStageCancel()
    {
        var result = await _service.CancelOrder(new CancelRequest()
        {
            Id = 2
        }, _context);
        
        Assert.False(result.IsSuccess);
    }
    

    [Fact]
    public async Task TestFailCancel()
    {
        var result = await _service.CancelOrder(new CancelRequest()
        {
            Id = 4
        }, _context);
        
        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task TestGetStatusFail()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetStatusById(new GetStatusByIdRequest
        {
            Id = 0
        }, _context));
        
        Assert.Equal("0", ex.Message);
    }

    [Fact]
    public async Task TestGetStatusSent()
    {
        var result = await _service.GetStatusById(new GetStatusByIdRequest
        {
            Id = 1
        }, _context);
        
        Assert.Equal(Order.Types.OrderState.SentToCustomer, result.State);
    }
    
    [Fact]
    public async Task TestGetStatusDelivered()
    {
        var result = await _service.GetStatusById(new GetStatusByIdRequest
        {
            Id = 2
        }, _context);
        
        Assert.Equal(Order.Types.OrderState.Delivered, result.State);
    }

    [Fact]
    public async Task TestGetRegions()
    {
        var result = await _service.GetRegions(new Empty(), _context);
        
        Assert.True(result.Regions.Any());
    }

    [Fact]
    public async Task TestGetOrdersRegionsFail()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetOrders(new GetOrdersRequest
        {
            RegionFilter = { "1", "0" },
            OrderTypeFilter = Order.Types.OrderType.UndefinedType,
            Page = new PagingRequest
            {
                SkipCount = 10,
                TakeCount = 40
            },
            Sort = SortType.None,
            SortField = Order.Types.SortField.NoneField
        }, _context));
        
        Assert.Equal("1, 0", ex.Message);
    }

    [Fact]
    public async Task TestGetOrders()
    {
        var result = await _service.GetOrders(new GetOrdersRequest
        {
            RegionFilter = { "1", "2" },
            OrderTypeFilter = Order.Types.OrderType.UndefinedType,
            Page = new PagingRequest
            {
                SkipCount = 10,
                TakeCount = 40
            },
            Sort = SortType.None,
            SortField = Order.Types.SortField.NoneField
        }, _context);

        Assert.NotNull(result.Orders);
    }

    [Fact]
    public async Task TestGetOrdersAggregationFail()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetOrdersAggregation(new GetOrdersAggregationRequest
        {
            Regions = { "0", "1" },
            FromDate = DateTime.UtcNow.ToTimestamp()
        }, _context));
        
        Assert.Equal("0, 1", ex.Message);
    }

    [Fact]
    public async Task TestGetOrdersAggregation()
    {
        var result = await _service.GetOrdersAggregation(new GetOrdersAggregationRequest
        {
            Regions = { "1", "2" },
            FromDate = DateTime.UtcNow.ToTimestamp()
        }, _context);

        Assert.NotNull(result.Aggregations);
    }

    [Fact]
    public async Task TestGetCustomerOrdersFail()
    {
        var request = new GetCustomerOrdersRequest
        {
            CustomerId = 0,
            Page = new PagingRequest
            {
                SkipCount = 0,
                TakeCount = 0
            },
            From = Timestamp.FromDateTime(DateTime.UtcNow)
        };
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetCustomerOrders(request, _context));
        
        Assert.Equal("0", ex.Message);
    }

    [Fact]
    public async Task TestGetCustomerOrders()
    {
        var request = new GetCustomerOrdersRequest
        {
            CustomerId = 1,
            Page = new PagingRequest
            {
                SkipCount = 0,
                TakeCount = 100
            },
            From = Timestamp.FromDateTime(DateTime.UtcNow)
        };
        var result = await _service.GetCustomerOrders(request, _context);
        
        Assert.NotNull(result);
    }
}