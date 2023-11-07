using FakeItEasy;
using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Application.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Application.Services.Impl;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Order = Ozon.Route256.Practice.OrdersService.Domain.Models.Order;

namespace Ozon.Route256.Practice.OrdersService.Test;

public sealed class OrdersGrpcServiceTests
{
    private readonly OrderService _orderService;
    private readonly RegionService _regionService;

    public OrdersGrpcServiceTests()
    {
        var regionRepository = FakeRegionRepository();
        var orderRepository = FakeOrderRepository();
        var addressRepository = FakeAddressRepository();
        var logisticsRepository = FakeLogisticsRepository();
        var customerRepository = FakeCustomerRepository();

        _orderService = new OrderService(
            regionRepository,
            orderRepository,
            addressRepository,
            logisticsRepository,
            customerRepository);
        _regionService = new RegionService(
            regionRepository);
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

    private Region CreateRegionDto(long id)
    {
        return new Region(id, id.ToString(), id, id);
    }

    private Region CreateRegionDto(string name)
    {
        var id = long.Parse(name);
        return new Region(id, name, id, id);
    }
    
    private IOrderRepository FakeOrderRepository()
    {
        var fake = A.Fake<IOrderRepository>();

        A.CallTo(() => fake.GetById(A<long>._, A<CancellationToken>._))
            .ReturnsLazily((long id, CancellationToken token) => CreateOrderDto(id));
        A.CallTo(() => fake.GetById(A<long>._, A<CancellationToken>._))
            .WhenArgumentsMatch((long id, CancellationToken token) => id <= 0 || id > 100)
            .ThrowsAsync((long id, CancellationToken token) => new NotFoundException(id.ToString()));

        A.CallTo(() => fake.GetAll(A<OrderRepositoryRequest>._, A<CancellationToken>._))
            .ReturnsLazily((OrderRepositoryRequest request, CancellationToken token) => GetOrders(request));

        A.CallTo(() => fake.GetByCustomerId(A<OrderByCustomerRepositoryRequest>._, A<CancellationToken>._))
            .ReturnsLazily((OrderByCustomerRepositoryRequest request, CancellationToken token) => GetOrders(request));

        A.CallTo(() => fake.UpdateOrderStatus(A<long>._, A<OrderState>._, A<CancellationToken>._))
            .ReturnsLazily((long id, OrderState state, CancellationToken token) => Task.CompletedTask);
        A.CallTo(() => fake.UpdateOrderStatus(A<long>._, A<OrderState>._, A<CancellationToken>._))
            .WhenArgumentsMatch((long id, OrderState state, CancellationToken token) => id <= 0 || id > 100)
            .ThrowsAsync((long id, OrderState state, CancellationToken token) => new NotFoundException(id.ToString()));

        return fake;
    }

    private Order CreateOrderDto(long id)
    {
        return new Order(
            id,
            (int)(id * 2),
            id * (decimal)1.5,
            id * (decimal)3.5,
            (OrderType)(id % 2 + 1),
            (OrderState)(id % 5 + 1),
            id,
            id,
            DateTime.Now);
    }

    private Order[] GetOrders(OrderRepositoryRequest repositoryRequest)
    {
        return Enumerable.Range(1, 100)
            .Select(i => CreateOrderDto(i + repositoryRequest.SkipCount))
            .Where(i => i.Type == repositoryRequest.OrderType)
            .Where(i => repositoryRequest.Regions.Contains(i.RegionFromId))
            .Take((int)repositoryRequest.TakeCount)
            .ToArray();
    }

    private Order[] GetOrders(OrderByCustomerRepositoryRequest byCustomerRepositoryRequest)
    {
        return Enumerable.Range(1, 100)
            .Where(i => i <= 100)
            .Select(i => CreateOrderDto(i + byCustomerRepositoryRequest.SkipCount))
            .Where(i => i.CustomerId == byCustomerRepositoryRequest.CustomerId)
            .Take((int)byCustomerRepositoryRequest.TakeCount)
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

    private Address CreateAddressDto(long id)
    {
        return new Address(
            id + 1,
            id % 3 + 1,
            id,
            id + 2,
            id.ToString() + "city",
            id.ToString() + "street",
            id.ToString() + "building",
            id.ToString() + "apartment",
            id / (decimal)long.MaxValue * 180,
            id / (decimal)long.MaxValue * 90);
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

    private CancelOrderResponse CreateCancelResultDto(long id)
    {
        return new CancelOrderResponse(id % 2 == 1, id % 2 == 1 ? "" : "Error");
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

    private Customer CreateCustomerDto(long id)
    {
        return new Customer((int)id, id.ToString() + "first", id.ToString() + "last", "+7-(999)-902-09-90",
            id.ToString() + "@ru.ru");
    }
    
    [Fact]
    public async Task TestCancelFail()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _orderService.CancelOrder(0, CancellationToken.None));
        
        Assert.Equal("0", ex.Message);
    }
    
    [Fact]
    public async Task TestSuccessCancel()
    {
        var result = await _orderService.CancelOrder(1, CancellationToken.None);
        
        Assert.True(result.Success);
    }
    
    [Fact]
    public async Task TestLastStageCancel()
    {
        var result = await _orderService.CancelOrder(2, CancellationToken.None);
        
        Assert.False(result.Success);
    }
    

    [Fact]
    public async Task TestFailCancel()
    {
        var result = await _orderService.CancelOrder(4, CancellationToken.None);
        
        Assert.False(result.Success);
    }

    [Fact]
    public async Task TestGetStatusFail()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetStatusById(0, CancellationToken.None));
        
        Assert.Equal("0", ex.Message);
    }

    [Fact]
    public async Task TestGetStatusSent()
    {
        var result = await _orderService.GetStatusById(1, CancellationToken.None);
        
        Assert.Equal(OrderState.SentToCustomer, result);
    }
    
    [Fact]
    public async Task TestGetStatusDelivered()
    {
        var result = await _orderService.GetStatusById(2, CancellationToken.None);
        
        Assert.Equal(OrderState.Delivered, result);
    }

    [Fact]
    public async Task TestGetRegions()
    {
        var result = await _regionService.GetRegions(CancellationToken.None);
        
        Assert.True(result.Any());
    }

    [Fact]
    public async Task TestGetOrdersRegionsFail()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetOrders(new OrdersRequest(
            Regions: new [] { "1", "0" },
            OrderType: OrderType.Undefined,
            SkipCount: 10,
            TakeCount: 40,
            SortingType: SortingType.None,
            OrderField: OrderField.NoneField,
            From: DateTime.Now), CancellationToken.None));
        
        Assert.Equal("1, 0", ex.Message);
    }

    [Fact]
    public async Task TestGetOrders()
    {
        var result = await _orderService.GetOrders(new OrdersRequest(
            Regions: new [] { "1", "2" },
            OrderType: OrderType.Undefined,
            SkipCount: 10,
            TakeCount: 40,
            SortingType: SortingType.None,
            OrderField: OrderField.NoneField,
            From: DateTime.Now), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task TestGetOrdersAggregationFail()
    {
        var ex = await Assert.ThrowsAsync<NotFoundException>(() =>
            _orderService.GetOrdersAggregation(new OrdersAggregationRequest(new[] { "0", "1" }, DateTime.Now), CancellationToken.None));
        
        Assert.Equal("0, 1", ex.Message);
    }

    [Fact]
    public async Task TestGetOrdersAggregation()
    {
        var result = await _orderService.GetOrdersAggregation(new OrdersAggregationRequest(new[] { "1", "2" }, DateTime.Now), CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public async Task TestGetCustomerOrdersFail()
    {
        var request = new CustomerOrdersRequest(
            CustomerId: 0,
            SkipCount: 0,
            TakeCount: 0,
            From: DateTime.UtcNow);
        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _orderService.GetCustomerOrders(request, CancellationToken.None));
        
        Assert.Equal("0", ex.Message);
    }

    [Fact]
    public async Task TestGetCustomerOrders()
    {
        var request = new CustomerOrdersRequest(
            CustomerId: 1,
            SkipCount: 0,
            TakeCount: 100,
            From: DateTime.UtcNow
        );
        var result = await _orderService.GetCustomerOrders(request, CancellationToken.None);
        
        Assert.NotNull(result);
    }
}