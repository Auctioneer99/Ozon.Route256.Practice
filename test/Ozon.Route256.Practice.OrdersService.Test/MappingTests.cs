using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders.Models;
using Ozon.Route256.Practice.OrdersService.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Test;

public sealed class MappingTests
{
    [Fact]
    public void OrderStateTest()
    {
        var model = OrderState.Cancelled;

        Assert.Equal(Grpc.Orders.Order.Types.OrderState.Cancelled, model.FromDto());
    }

    [Fact]
    public void OrderTypeTest()
    {
        var model = OrderType.Mobile;

        Assert.Equal(Grpc.Orders.Order.Types.OrderType.Mobile, model.FromDto());
    }

    [Fact]
    public void OrderTest()
    {
        var order = new Order(
            1,
            2,
            123,
            432,
            (int)OrderType.Web,
            (int)OrderState.Cancelled,
            1,
            1,
            DateTime.UtcNow);

        var region = new Region(1, "Moscow", (decimal)55.7522, (decimal)37.6156);

        var customer = new Customer(1, "asd", "dsa", "123123123", "a@a.a");

        var address = new Grpc.Orders.Order.Types.Address
        {
            Region = "Novosibirsk",
            City = "asd",
            Street = "fdsf",
            Building = "asdasd",
            Apartment = "324234",
            Latitude = 123.543,
            Longitude = 23.2
        };

        var mapped = order.FromDto(region, customer, address);

        Assert.Equal(order.Id, mapped.Id);
        Assert.Equal(order.Count, mapped.Count);
        Assert.Equal(order.TotalSum, (decimal)mapped.TotalSum);
        Assert.Equal(order.TotalWeight, (decimal)mapped.TotalWeight);
        Assert.Equal(Grpc.Orders.Order.Types.OrderType.Web, mapped.Type);
        Assert.Equal(Grpc.Orders.Order.Types.OrderState.Cancelled, mapped.State);
        Assert.Equal(order.CreatedAt, mapped.CreatedAt.ToDateTime());
        Assert.Equal(region.Name, mapped.RegionFrom);
        Assert.Equal(address.Apartment, mapped.OrderAddress.Apartment);
        Assert.Equal(address.Building, mapped.OrderAddress.Building);
        Assert.Equal(address.Street, mapped.OrderAddress.Street);
        Assert.Equal(address.City, mapped.OrderAddress.City);
        Assert.Equal(address.Region, mapped.OrderAddress.Region);
        Assert.Equal(address.Latitude, mapped.OrderAddress.Latitude);
        Assert.Equal(address.Longitude, mapped.OrderAddress.Longitude);
        Assert.Equal($"{customer.FirstName} {customer.LastName}", mapped.CustomerName);
        Assert.Equal(customer.MobileNumber, mapped.Phone);
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

        var mapped = model.FromDto(region);

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

        var mapped = model.FromDto();

        Assert.Equal(model.Success, mapped.IsSuccess);
        Assert.Equal(model.Error, mapped.Error);
    }

    [Fact]
    public void OrderFieldTest()
    {
        var model = Grpc.Orders.Order.Types.SortField.CreatedAt;

        Assert.Equal(OrderField.CreatedAt, model.ToDto());
    }

    [Fact]
    public void SortTypeTest()
    {
        var model = Grpc.Orders.SortType.Descending;

        Assert.Equal(SortingType.Descending, model.ToDto());
    }

    [Fact]
    public void TestOrderType()
    {
        var model = Grpc.Orders.Order.Types.OrderType.Web;

        Assert.Equal(OrderType.Web, model.ToDto());
    }

    [Fact]
    public void CustomerTest()
    {
        var model = new Grpc.Customers.Customer
        {
            Id = 0,
            FirstName = "fsadf",
            LastName = "fdsdfdsgf",
            MobileNumber = "123432",
            Email = "a4@fds.w",
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

        var mapped = model.ToDto();

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

        var regions = new long[] { 1, 2 };
        var mapped = model.ToDto(regions);
        
        Assert.Equal(model.Sort.ToDto(), mapped.SortingType);
        Assert.Equal(model.SortField.ToDto(), mapped.OrderField);
        Assert.Equal(model.Page.TakeCount, mapped.TakeCount);
        Assert.Equal(model.Page.SkipCount, mapped.SkipCount);
        Assert.Equal(model.OrderTypeFilter.ToDto(), mapped.OrderType);
        Assert.Equal(regions, mapped.Regions);
    }

    [Fact]
    public void CancelResultTest()
    {
        var model = new Grpc.LogisticsSimulator.CancelResult
        {
            Success = true,
            Error = "asd"
        };

        var mapped = model.ToDto();
        
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

        var mapped = model.ToDto();

        Assert.Equal(model.CustomerId, mapped.CustomerId);
        Assert.Equal(model.From.ToDateTime(), mapped.From);
        Assert.Equal(model.Page.SkipCount, mapped.SkipCount);
        Assert.Equal(model.Page.TakeCount, mapped.TakeCount);
    }

    [Fact]
    public void OrderKafkaStateTest()
    {
        var state = Infrastructure.Kafka.Consumer.OrdersEvents.Models.OrderState.Created;

        var mapped = state.ToDto();
        
        Assert.Equal(OrderState.Created, mapped);
    }

    [Fact]
    public void OrderSourceTest()
    {
        var source = OrderSource.Mobile;

        var mapped = source.ToDto();
        
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
        var mapped = order.ToDto(2, date);
        
        Assert.Equal(order.Id, mapped.Id);
        Assert.Equal(order.Goods.Sum(g => g.Quantity), mapped.Count);
        Assert.Equal(order.Goods.Sum(g => g.Price), (double)mapped.TotalSum);
        Assert.Equal(order.Goods.Sum(g => g.Weight), (double)mapped.TotalWeight);
        Assert.Equal((int)OrderType.Web, mapped.Type);
        Assert.Equal((int)OrderState.Created, mapped.State);
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

        var mapped = address.ToDto(3, 2, 1);
        
        Assert.Equal(0, mapped.Id);
        Assert.Equal(3, mapped.RegionId);
        Assert.Equal(address.City, mapped.City);
        Assert.Equal(address.Street, mapped.Street);
        Assert.Equal(address.Building, mapped.Building);
        Assert.Equal(address.Apartment, mapped.Apartment);
        Assert.Equal(address.Latitude, (double)mapped.Latitude);
        Assert.Equal(address.Longitude, (double)mapped.Longitude);
    }
}