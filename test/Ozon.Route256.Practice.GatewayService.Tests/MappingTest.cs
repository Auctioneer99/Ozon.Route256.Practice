using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.GatewayService.Models;
using Ozon.Route256.Practice.GatewayService.Services.Mapper;

namespace Ozon.Route256.Practice.GatewayService.Tests;

public sealed class MappingTest
{
    [Fact]
    public void OrderTypeTest()
    {
        var model = OrderType.Web;
        
        Assert.True(model.FromDto().ToDto() == OrderType.Web);
    }

    [Fact]
    public void OrderStateTest()
    {
        var model = OrderState.Created;
        
        Assert.True(model.FromDto().ToDto() == OrderState.Created);
    }

    [Fact]
    public void OrderTest()
    {
        var model = new Grpc.Orders.Order
        {
            Id = 2,
            Count = 3,
            TotalSum = 4,
            TotalWeight = 5,
            Type = Grpc.Orders.Order.Types.OrderType.Web,
            CreatedAt = DateTime.Now.ToUniversalTime().ToTimestamp(),
            RegionFrom = "RU",
            State = Grpc.Orders.Order.Types.OrderState.Cancelled,
            CustomerName = "client",
            OrderAddress = new Grpc.Orders.Order.Types.Address
            {
                Region = "RU",
                City = "Ty",
                Street = "Main",
                Building = "23",
                Apartment = "33",
                Latitude = 123,
                Longitude = 321
            },
            Phone = "phone"
        };

        var mapped = model.ToDto().FromDto();
        
        Assert.Equal(model.Id, mapped.Id);
        Assert.Equal(model.Count, mapped.Count);
        Assert.Equal(model.TotalSum, mapped.TotalSum);
        Assert.Equal(model.TotalWeight, mapped.TotalWeight);
        Assert.Equal(model.Type, mapped.Type);
        Assert.Equal(model.CreatedAt, mapped.CreatedAt);
        Assert.Equal(model.RegionFrom, mapped.RegionFrom);
        Assert.Equal(model.State, mapped.State);
        Assert.Equal(model.CustomerName, mapped.CustomerName);
        Assert.Equal(model.OrderAddress.Region, mapped.OrderAddress.Region);
        Assert.Equal(model.OrderAddress.City, mapped.OrderAddress.City);
        Assert.Equal(model.OrderAddress.Street, mapped.OrderAddress.Street);
        Assert.Equal(model.OrderAddress.Building, mapped.OrderAddress.Building);
        Assert.Equal(model.OrderAddress.Apartment, mapped.OrderAddress.Apartment);
        Assert.Equal(model.OrderAddress.Latitude, mapped.OrderAddress.Latitude);
        Assert.Equal(model.OrderAddress.Longitude, mapped.OrderAddress.Longitude);
        Assert.Equal(model.Phone, mapped.Phone);
    }

    [Fact]
    public void AddressTest()
    {
        var model = new Address
        {
            Region = "RU",
            City = "Ty",
            Street = "Main",
            Building = "23",
            Apartment = "33",
            Latitude = 123,
            Longitude = 321
        };

        var mapped = model.FromDtoToCustomer().ToDto();
        
        Assert.Equal(model.Region, mapped.Region);
        Assert.Equal(model.City, mapped.City);
        Assert.Equal(model.Street, mapped.Street);
        Assert.Equal(model.Building, mapped.Building);
        Assert.Equal(model.Apartment, mapped.Apartment);
        Assert.Equal(model.Latitude, mapped.Latitude);
        Assert.Equal(model.Longitude, mapped.Longitude);
        
        mapped = model.FromDtoToOrder().ToDto();
        
        Assert.Equal(model.Region, mapped.Region);
        Assert.Equal(model.City, mapped.City);
        Assert.Equal(model.Street, mapped.Street);
        Assert.Equal(model.Building, mapped.Building);
        Assert.Equal(model.Apartment, mapped.Apartment);
        Assert.Equal(model.Latitude, mapped.Latitude);
        Assert.Equal(model.Longitude, mapped.Longitude);
    }

    [Fact]
    public void PagingRequestTest()
    {
        var model = new PagingRequest
        {
            SkipCount = 1,
            TakeCount = 2
        };

        var mapped = model.FromDto();
        
        Assert.Equal(model.SkipCount, mapped.SkipCount);
        Assert.Equal(model.TakeCount, mapped.TakeCount);
    }

    [Fact]
    public void CancelRequestTest()
    {
        var model = new CancelRequest
        {
            Id = 3
        };

        var mapped = model.FromDto();
        
        Assert.Equal(model.Id, mapped.Id);
    }

    [Fact]
    public void SortTypeTest()
    {
        var model = SortType.Ascending;
        Assert.Equal(Grpc.Orders.SortType.Ascending, model.FromDto());
    }

    [Fact]
    public void SortFieldTest()
    {
        var model = OrderFilterField.Count;
        Assert.Equal(Grpc.Orders.Order.Types.SortField.Count, model.FromDto());
    }

    [Fact]
    public void OrdersRequestTest()
    {
        var model = new OrdersRequest
        {
            RegionFilter = new string[]
            {
                "RU"
            },
            OrderTypeFilter = OrderType.Web,
            Page = new()
            {
                SkipCount = 2,
                TakeCount = 3
            },
            Sort = SortType.Ascending,
            SortField = OrderFilterField.Count
        };

        var mapped = model.FromDto();
        
        Assert.Equal(model.RegionFilter, mapped.RegionFilter);
        Assert.Equal(Grpc.Orders.Order.Types.OrderType.Web, mapped.OrderTypeFilter);
        Assert.Equal(model.Page.SkipCount, mapped.Page.SkipCount);
        Assert.Equal(model.Page.TakeCount, mapped.Page.TakeCount);
        Assert.Equal(Grpc.Orders.SortType.Ascending, mapped.Sort);
        Assert.Equal(Grpc.Orders.Order.Types.SortField.Count, mapped.SortField);
    }

    [Fact]
    public void OrdersAggregationRequestTest()
    {
        var model = new OrdersAggregationRequest
        {
            FromDate = DateTime.Now,
            Regions = new string[]
            {
                "RU"
            }
        };

        var mapped = model.FromDto();
        
        Assert.Equal(model.FromDate.ToUniversalTime(), mapped.FromDate.ToDateTime());
        Assert.Equal(model.Regions, mapped.Regions);
    }

    [Fact]
    public void CustomerOrdersRequestTest()
    {
        var model = new CustomerOrdersRequest
        {
            CustomerId = 2,
            From = DateTime.Now,
            Page = new()
            {
                SkipCount = 2,
                TakeCount = 3
            }
        };

        var mapped = model.FromDto();
        
        Assert.Equal(model.CustomerId, mapped.CustomerId);
        Assert.Equal(model.From.ToUniversalTime(), mapped.From.ToDateTime());
        Assert.Equal(model.Page.SkipCount, mapped.Page.SkipCount);
        Assert.Equal(model.Page.TakeCount, mapped.Page.TakeCount);
    }

    [Fact]
    public void CustomerTest()
    {
        var model = new Grpc.Customers.Customer()
        {
            Id = 2,
            FirstName = "asd",
            LastName = "dsa",
            MobileNumber = "asd",
            Email = "qweqw",
            DefaultAddress = new()
            {
                Region = "asd",
                City = "cxvcx",
                Street = "213d",
                Building = "asdas",
                Apartment = "54654",
                Latitude = 5647,
                Longitude = 234324
            }
        };

        var mapped = model.ToDto();
        
        Assert.Equal(model.Id, mapped.Id);
        Assert.Equal(model.FirstName, mapped.FirstName);
        Assert.Equal(model.LastName, mapped.LastName);
        Assert.Equal(model.MobileNumber, mapped.MobileNumber);
        Assert.Equal(model.Email, mapped.Email);
        Assert.Equal(model.DefaultAddress.Region, mapped.DefaultAddress.Region);
        Assert.Equal(model.DefaultAddress.City, mapped.DefaultAddress.City);
        Assert.Equal(model.DefaultAddress.Street, mapped.DefaultAddress.Street);
        Assert.Equal(model.DefaultAddress.Building, mapped.DefaultAddress.Building);
        Assert.Equal(model.DefaultAddress.Apartment, mapped.DefaultAddress.Apartment);
        Assert.Equal(model.DefaultAddress.Latitude, mapped.DefaultAddress.Latitude);
        Assert.Equal(model.DefaultAddress.Longitude, mapped.DefaultAddress.Longitude);
    }

    [Fact]
    public void GetCustomersResponseTest()
    {
        var model = new Grpc.Customers.GetCustomersResponse()
        {
            Customers = { }
        };

        var mapped = model.ToDto();
        
        Assert.Equal(model.Customers.Count, mapped.Customers.Length);
    }

    [Fact]
    public void CancelResponseTest()
    {
        var model = new Grpc.Orders.CancelResponse
        {
            IsSuccess = true,
            Error = "asd"
        };

        var mapped = model.ToDto();
        
        Assert.Equal(model.IsSuccess, mapped.IsSuccess);
        Assert.Equal(model.Error, mapped.Error);
    }

    [Fact]
    public void GetStatusByIdResponse()
    {
        var model = new Grpc.Orders.GetStatusByIdResponse
        {
            State = Grpc.Orders.Order.Types.OrderState.Created
        };

        var mapped = model.ToDto();
        
        Assert.Equal("Created", mapped.Status);
    }

    [Fact]
    public void GetRegionsResponseTest()
    {
        var model = new Grpc.Orders.GetRegionsResponse()
        {
            Regions = { "RU" }
        };

        var mapped = model.ToDto();
        
        Assert.Equal(model.Regions[0], mapped.Regions[0]);
    }

    [Fact]
    public void GetOrdersResponseTest()
    {
        var model = new Grpc.Orders.GetOrdersResponse()
        {
            Orders = { }
        };

        var mapped = model.ToDto();

        Assert.Equal(model.Orders.Count, mapped.Orders.Length);
    }

    [Fact]
    public void GetOrdersAggregationResponseEntryTest()
    {
        var model = new Grpc.Orders.GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry
        {
            Region = "asda",
            OrdersCount = 321,
            TotalOrdersSum = 435,
            TotalOrdersWeight = 123,
            UniqueCustomersCount = 534654
        };

        var mapped = model.ToDto();
        
        Assert.Equal(model.Region, mapped.Region);
        Assert.Equal(model.OrdersCount, mapped.OrdersCount);
        Assert.Equal(model.TotalOrdersSum, mapped.TotalOrdersSum);
        Assert.Equal(model.TotalOrdersWeight, mapped.TotalOrdersWeight);
        Assert.Equal(model.UniqueCustomersCount, mapped.UniqueClientsCount);
    }

    [Fact]
    public void GetOrdersAggregationResponseTest()
    {
        var model = new Grpc.Orders.GetOrdersAggregationResponse()
        {
            Aggregations = { }
        };

        var mapped = model.ToDto();
        
        Assert.Equal(model.Aggregations.Count, mapped.Aggregations.Length);
    }

    [Fact]
    public void GetCustomerOrdersResponseTest()
    {
        var model = new Grpc.Orders.GetCustomerOrdersResponse()
        {
            Orders = { }
        };

        var mapped = model.ToDto();
        
        Assert.Equal(model.Orders.Count, mapped.Orders.Length);
    }
}