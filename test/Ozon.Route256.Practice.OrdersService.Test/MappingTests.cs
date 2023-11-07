using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;
using Ozon.Route256.Practice.OrdersService.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Test;

public sealed class MappingTests
{
    [Fact]
    public void OrderStateTest()
    {
        var model = OrderState.Cancelled;

        Assert.Equal(Grpc.Orders.Order.Types.OrderState.Cancelled, model.ToHost());
    }

    [Fact]
    public void OrderTypeTest()
    {
        var model = OrderType.Mobile;

        Assert.Equal(Grpc.Orders.Order.Types.OrderType.Mobile, model.ToHost());
    }
    
    [Fact]
    public void AddressTest()
    {
        var model = new Address(
            1,
            2,
            3,
            4,
            "asdasd",
            "sadasd",
            "dsfdsf",
            "asdasdasd",
            (decimal)45.6546,
            (decimal)43.3434);

        var region = new Region(2, "Moscow", (decimal)55.7522, (decimal)37.6156);

        var mapped = model.ToHost(region);

        Assert.Equal(region.Name, mapped.Region);
        Assert.Equal(model.City, mapped.City);
        Assert.Equal(model.Street, mapped.Street);
        Assert.Equal(model.Building, mapped.Building);
        Assert.Equal(model.Apartment, mapped.Apartment);
        Assert.Equal(model.Latitude, (decimal)mapped.Latitude);
        Assert.Equal(model.Longitude, (decimal)mapped.Longitude);
    }

    [Fact]
    public void CancelResponseTest()
    {
        var model = new CancelOrderResponse(true, "Error");

        var mapped = model.ToHost();

        Assert.Equal(model.Success, mapped.IsSuccess);
        Assert.Equal(model.Error, mapped.Error);
    }

    [Fact]
    public void OrderFieldTest()
    {
        var model = Grpc.Orders.Order.Types.SortField.CreatedAt;

        Assert.Equal(OrderField.CreatedAt, model.ToApplication());
    }

    [Fact]
    public void SortTypeTest()
    {
        var model = Grpc.Orders.SortType.Descending;

        Assert.Equal(SortingType.Descending, model.ToApplication());
    }

    [Fact]
    public void TestOrderType()
    {
        var model = Grpc.Orders.Order.Types.OrderType.Web;

        Assert.Equal(OrderType.Web, model.ToApplication());
    }

    [Fact]
    public void CustomerTest()
    {
        var model = new Grpc.Customers.Customer
        {
            Id = 0,
            FirstName = "fsadf",
            LastName = "fdsdfdsgf",
            MobileNumber = "+7-(999)-902-09-90",
            Email = "a4@fds.wu",
            DefaultAddress = new Grpc.Customers.Address
            {
                Region = "sad",
                City = "vxcv",
                Street = "ervb",
                Building = "sfdsf",
                Apartment = "xvxv",
                Latitude = 54.34,
                Longitude = 65.34
            }
        };

        var mapped = model.ToDomain();

        Assert.Equal(model.Id, mapped.Id);
        Assert.Equal(model.FirstName, mapped.FirstName);
        Assert.Equal(model.LastName, mapped.LastName);
        Assert.Equal(model.MobileNumber, mapped.MobileNumber);
        Assert.Equal(model.Email, mapped.Email);
    }

    [Fact]
    public void OrderRequestTest()
    {
        var model = new Grpc.Orders.GetOrdersRequest
        {
            OrderTypeFilter = Grpc.Orders.Order.Types.OrderType.Web,
            Page = new Grpc.Orders.PagingRequest
            {
                SkipCount = 32,
                TakeCount = 41
            },
            Sort = Grpc.Orders.SortType.Ascending,
            SortField = Grpc.Orders.Order.Types.SortField.Count
        };

        var mapped = model.ToApplication();
        
        Assert.Equal(model.Sort.ToApplication(), mapped.SortingType);
        Assert.Equal(model.SortField.ToApplication(), mapped.OrderField);
        Assert.Equal(model.Page.TakeCount, mapped.TakeCount);
        Assert.Equal(model.Page.SkipCount, mapped.SkipCount);
        Assert.Equal(model.OrderTypeFilter.ToApplication(), mapped.OrderType);
    }

    [Fact]
    public void CancelResultTest()
    {
        var model = new Grpc.LogisticsSimulator.CancelResult
        {
            Success = true,
            Error = "asd"
        };

        var mapped = model.ToApplication();
        
        Assert.Equal(model.Success, mapped.Success);
        Assert.Equal(model.Error, mapped.Error);
    }

    [Fact]
    public void OrderRequestByCustomerTest()
    {
        var model = new Grpc.Orders.GetCustomerOrdersRequest
        {
            CustomerId = 2,
            From = DateTime.UtcNow.ToTimestamp(),
            Page = new Grpc.Orders.PagingRequest
            {
                SkipCount = 2,
                TakeCount = 3
            }
        };

        var mapped = model.ToApplication();

        Assert.Equal(model.CustomerId, mapped.CustomerId);
        Assert.Equal(model.From.ToDateTime(), mapped.From);
        Assert.Equal(model.Page.SkipCount, mapped.SkipCount);
        Assert.Equal(model.Page.TakeCount, mapped.TakeCount);
    }

    [Fact]
    public void OrderKafkaStateTest()
    {
        var state = Infrastructure.Kafka.Consumer.OrdersEvents.Models.OrderState.Created;

        var mapped = state.ToDomain();
        
        Assert.Equal(OrderState.Created, mapped);
    }

    [Fact]
    public void OrderSourceTest()
    {
        var source = OrderSource.Mobile;

        var mapped = source.ToDomain();
        
        Assert.Equal(OrderType.Mobile, mapped);
    }

    [Fact]
    public void PreOrderTest()
    {
        var order = new PreOrder(
            1,
            OrderSource.Web,
            new PreCustomer(
                1,
                new PreAddress(
                    "Moscow",
                    "City",
                    "Street",
                    "Building",
                    "Apartment",
                    12.12,
                    23.32)
            ),
            new PreGood[1]
            {
                new PreGood(
                    1,
                    "Good",
                    2,
                    2,
                    2)
            });

        var date = DateTime.UtcNow;
        var mapped = order.ToDomain(2, date);
        
        Assert.Equal(order.Id, mapped.Id);
        Assert.Equal(order.Goods.Sum(g => g.Quantity), mapped.Count);
        Assert.Equal(order.Goods.Sum(g => g.Price), (double)mapped.TotalSum);
        Assert.Equal(order.Goods.Sum(g => g.Weight), (double)mapped.TotalWeight);
        Assert.Equal(OrderType.Web, mapped.Type);
        Assert.Equal(OrderState.Created, mapped.State);
        Assert.Equal(2, mapped.RegionFromId);
        Assert.Equal(order.Customer.Id, mapped.CustomerId);
        Assert.Equal(date, mapped.CreatedAt);
    }

    [Fact]
    public void PreAddressTest()
    {
        var address = new PreAddress(
            "Moscow",
            "City",
            "Street",
            "Building",
            "Apartment",
            20.3,
            54.3);

        var mapped = address.ToDomain(3, 2, 1);
        
        Assert.Equal(2, mapped.Id);
        Assert.Equal(3, mapped.RegionId);
        Assert.Equal(address.City, mapped.City);
        Assert.Equal(address.Street, mapped.Street);
        Assert.Equal(address.Building, mapped.Building);
        Assert.Equal(address.Apartment, mapped.Apartment);
        Assert.Equal(address.Latitude, (double)mapped.Latitude);
        Assert.Equal(address.Longitude, (double)mapped.Longitude);
    }
}