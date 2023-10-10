using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.Grpc.Orders;
using Ozon.Route256.Practice.OrdersService.Services.Validation;

namespace Ozon.Route256.Practice.OrdersService.Test;

public sealed class ValidatorTests
{
    private readonly CancelRequestValidator _cancelRequestValidator = new();
    private readonly GetCustomerOrdersRequestValidator _getClientOrdersRequestValidator = new();
    private readonly GetOrdersAggregationRequestValidator _getOrdersAggregationRequestValidator = new();
    private readonly GetOrdersRequestValidator _getOrdersRequestValidator = new();
    private readonly GetStatusByIdRequestValidator _getStatusByIdRequestValidator = new();
    
    [Fact]
    public void CancelRequestValidatorTest()
    {
        Assert.True(_cancelRequestValidator.Validate(new CancelRequest()
        {
            Id = 0
        }));
        Assert.False(_cancelRequestValidator.Validate(new CancelRequest()
        {
            Id = -1
        }));
    }
    
    [Fact]
    public void GetClientOrdersRequestValidatorTest()
    {
        Assert.True(_getClientOrdersRequestValidator.Validate(new GetCustomerOrdersRequest
        {
            CustomerId = 0,
            From = DateTime.Now.AddDays(-1).ToUniversalTime().ToTimestamp(),
            Page = new() { TakeCount = 1 }
        }));
        Assert.False(_getClientOrdersRequestValidator.Validate(new GetCustomerOrdersRequest
        {
            CustomerId = 0,
            From = DateTime.Now.AddDays(-1).ToUniversalTime().ToTimestamp(),
            Page = null
        }));
        Assert.False(_getClientOrdersRequestValidator.Validate(new GetCustomerOrdersRequest()
        {
            CustomerId = 0,
            From = DateTime.Now.AddDays(-1).ToUniversalTime().ToTimestamp(),
            Page = new()
        }));
        Assert.False(_getClientOrdersRequestValidator.Validate(new GetCustomerOrdersRequest()
        {
            CustomerId = 0,
            From = null,
            Page = new() { TakeCount = 1 }
        }));
        Assert.False(_getClientOrdersRequestValidator.Validate(new GetCustomerOrdersRequest()
        {
            CustomerId = 0,
            From = DateTime.Now.AddDays(-1).ToUniversalTime().ToTimestamp(),
            Page = new() { TakeCount = 1, SkipCount = -1 }
        }));
        Assert.False(_getClientOrdersRequestValidator.Validate(new GetCustomerOrdersRequest()
        {
            CustomerId = -1,
            From = DateTime.Now.AddDays(-1).ToUniversalTime().ToTimestamp(),
            Page = new() { TakeCount = 1 }
        }));
    }

    [Fact]
    public void GetOrdersAggregationRequestValidatorTest()
    {
        Assert.True(_getOrdersAggregationRequestValidator.Validate(new GetOrdersAggregationRequest
        {
            FromDate = DateTime.Now.AddDays(-1).ToUniversalTime().ToTimestamp(),
            Regions = { "RU" }
        }));
        Assert.False(_getOrdersAggregationRequestValidator.Validate(new GetOrdersAggregationRequest
        {
            FromDate = null,
            Regions = { "RU" }
        }));
        Assert.False(_getOrdersAggregationRequestValidator.Validate(new GetOrdersAggregationRequest
        {
            FromDate = DateTime.Now.AddDays(-1).ToUniversalTime().ToTimestamp(),
            Regions = {  }
        }));
    }

    [Fact]
    public void GetOrdersRequestValidatorTest()
    {
        Assert.True(_getOrdersRequestValidator.Validate(new GetOrdersRequest
        {
            OrderTypeFilter = Order.Types.OrderType.FirstType,
            Page = new() { TakeCount = 1 },
            Sort = SortType.None,
            SortField = Order.Types.SortField.NoneField,
            RegionFilter = { "RU" }
        }));
        Assert.True(_getOrdersRequestValidator.Validate(new GetOrdersRequest
        {
            OrderTypeFilter = Order.Types.OrderType.FirstType,
            Page = new() { TakeCount = 1 },
            Sort = SortType.None,
            SortField = Order.Types.SortField.NoneField,
            RegionFilter = {  }
        }));
        Assert.False(_getOrdersRequestValidator.Validate(new GetOrdersRequest
        {
            OrderTypeFilter = Order.Types.OrderType.UndefinedType,
            Page = new() { TakeCount = 1 },
            Sort = SortType.None,
            SortField = Order.Types.SortField.NoneField,
            RegionFilter = { "RU" }
        }));
        Assert.False(_getOrdersRequestValidator.Validate(new GetOrdersRequest
        {
            OrderTypeFilter = Order.Types.OrderType.FirstType,
            Page = null,
            Sort = SortType.None,
            SortField = Order.Types.SortField.NoneField,
            RegionFilter = { "RU" }
        }));
        Assert.False(_getOrdersRequestValidator.Validate(new GetOrdersRequest
        {
            OrderTypeFilter = Order.Types.OrderType.FirstType,
            Page = new(),
            Sort = SortType.None,
            SortField = Order.Types.SortField.NoneField,
            RegionFilter = { "RU" }
        }));
    }

    [Fact]
    public void GetStatusByIdRequestValidatorTest()
    {
        Assert.True(_getStatusByIdRequestValidator.Validate(new GetStatusByIdRequest
        {
            Id = 0
        }));
        Assert.False(_getStatusByIdRequestValidator.Validate(new GetStatusByIdRequest
        {
            Id = -1
        }));
    }
}