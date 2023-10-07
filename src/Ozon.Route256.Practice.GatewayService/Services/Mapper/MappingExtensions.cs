using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.GatewayService.Services.Mapper;

public static class MappingExtensions
{
    public static Grpc.Orders.Order.Types.OrderState FromDto(this OrderState type)
    {
        return (Grpc.Orders.Order.Types.OrderState)type;
    }
    
    public static Grpc.Orders.Order FromDto(this Order dto)
    {
        return new Grpc.Orders.Order
        {
            Id = dto.Id,
            Count = dto.Count,
            TotalSum = dto.TotalSum,
            TotalWeight = dto.TotalWeight,
            Type = dto.Type.FromDto(),
            CreatedAt = dto.CreatedAt.ToUniversalTime().ToTimestamp(),
            RegionFrom = dto.RegionFrom,
            State = dto.State.FromDto(),
            CustomerName = dto.ClientName,
            OrderAddress = dto.OrderAddress.FromDtoToOrder(),
            Phone = dto.Phone
        };
    }

    public static Grpc.Customers.Address FromDtoToCustomer(this Address dto)
    {
        return new Grpc.Customers.Address
        {
            Region = dto.Region,
            City = dto.City,
            Street = dto.Street,
            Building = dto.Building,
            Apartment = dto.Apartment,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };
    }

    public static Grpc.Orders.Order.Types.Address FromDtoToOrder(this Address dto)
    {
        return new Grpc.Orders.Order.Types.Address
        {
            Region = dto.Region,
            City = dto.City,
            Street = dto.Street,
            Building = dto.Building,
            Apartment = dto.Apartment,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };
    }

    public static Grpc.Orders.PagingRequest FromDto(this PagingRequest dto)
    {
        return new Grpc.Orders.PagingRequest
        {
            SkipCount = dto.SkipCount,
            TakeCount = dto.TakeCount
        };
    }

    public static Grpc.Orders.CancelRequest FromDto(this CancelRequest dto)
    {
        return new Grpc.Orders.CancelRequest
        {
            Id = dto.Id
        };
    }

    public static Grpc.Orders.Order.Types.OrderType FromDto(this OrderType type)
    {
        return (Grpc.Orders.Order.Types.OrderType)type;
    }

    public static Grpc.Orders.SortType FromDto(this SortType type)
    {
        return (Grpc.Orders.SortType)type;
    }

    public static Grpc.Orders.Order.Types.SortField FromDto(this OrderFilterField type)
    {
        return (Grpc.Orders.Order.Types.SortField)type;
    }
    
    public static Grpc.Orders.GetOrdersRequest FromDto(this OrdersRequest dto)
    {
        return new Grpc.Orders.GetOrdersRequest
        {
            OrderTypeFilter = dto.OrderTypeFilter.FromDto(),
            Page = dto.Page.FromDto(),
            Sort = dto.Sort.FromDto(),
            SortField = dto.SortField.FromDto(),
            RegionFilter = { dto.RegionFilter }
        };
    }

    public static Grpc.Orders.GetOrdersAggregationRequest FromDto(this OrdersAggregationRequest dto)
    {
        return new Grpc.Orders.GetOrdersAggregationRequest
        {
            FromDate = dto.FromDate.ToUniversalTime().ToTimestamp(),
            Regions = { dto.Regions }
        };
    }

    public static Grpc.Orders.GetCustomerOrdersRequest FromDto(this CustomerOrdersRequest dto)
    {
        return new Grpc.Orders.GetCustomerOrdersRequest
        {
            CustomerId = dto.CustomerId,
            From = dto.From.ToUniversalTime().ToTimestamp(),
            Page = dto.Page.FromDto()
        };
    }
    
    public static OrderType ToDto(this Grpc.Orders.Order.Types.OrderType type)
    {
        return (OrderType)type;
    }

    public static OrderState ToDto(this Grpc.Orders.Order.Types.OrderState type)
    {
        return (OrderState)type;
    }

    public static Address ToDto(this Grpc.Customers.Address model)
    {
        return new Address
        {
            Region = model.Region,
            City = model.City,
            Street = model.Street,
            Building = model.Building,
            Apartment = model.Apartment,
            Latitude = model.Latitude,
            Longitude = model.Longitude
        };
    }

    public static Address ToDto(this Grpc.Orders.Order.Types.Address model)
    {
        return new Address
        {
            Region = model.Region,
            City = model.City,
            Street = model.Street,
            Building = model.Building,
            Apartment = model.Apartment,
            Latitude = model.Latitude,
            Longitude = model.Longitude
        };
    }
    
    public static Customer ToDto(this Grpc.Customers.Customer model)
    {
        return new Customer
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            MobileNumber = model.MobileNumber,
            Email = model.Email,
            DefaultAddress = model.DefaultAddress.ToDto(),
            Addresses = model.Addresses.Select(a => a.ToDto()).ToArray()
        };
    }
    
    public static CustomersResponse ToDto(this Grpc.Customers.GetCustomersResponse model)
    {
        return new CustomersResponse
        {
            Customers = model.Customers.Select(c => c.ToDto()).ToArray()
        };
    }

    public static CancelResponse ToDto(this Grpc.Orders.CancelResponse model)
    {
        return new CancelResponse
        {
            IsSuccess = model.IsSuccess,
            Error = model.Error
        };
    }

    public static StatusResponse ToDto(this Grpc.Orders.GetStatusByIdResponse model)
    {
        return new StatusResponse
        {
            Status = model.State.ToString()
        };
    }

    public static RegionsResponse ToDto(this Grpc.Orders.GetRegionsResponse model)
    {
        return new RegionsResponse
        {
            Regions = model.Regions.ToArray()
        };
    }
    
    public static Order ToDto(this Grpc.Orders.Order model)
    {
        return new Order
        {
            Id = model.Id,
            Count = model.Count,
            TotalSum = model.TotalSum,
            TotalWeight = model.TotalWeight,
            Type = model.Type.ToDto(),
            CreatedAt = model.CreatedAt.ToDateTime(),
            RegionFrom = model.RegionFrom,
            State = model.State.ToDto(),
            ClientName = model.CustomerName,
            OrderAddress = model.OrderAddress.ToDto(),
            Phone = model.Phone
        };
    }
    
    public static OrdersResponse ToDto(this Grpc.Orders.GetOrdersResponse model)
    {
        return new OrdersResponse
        {
            Orders = model.Orders.Select(o => o.ToDto()).ToArray()
        };
    }

    public static OrdersAggregationResponse.OrdersAggregationResponseEntry ToDto(this Grpc.Orders.GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry model)
    {
        return new OrdersAggregationResponse.OrdersAggregationResponseEntry
        {
            Region = model.Region,
            OrdersCount = model.OrdersCount,
            TotalOrdersSum = model.TotalOrdersSum,
            TotalOrdersWeight = model.TotalOrdersWeight,
            UniqueClientsCount = model.UniqueCustomersCount
        };
    }
    
    public static OrdersAggregationResponse ToDto(this Grpc.Orders.GetOrdersAggregationResponse model)
    {
        return new OrdersAggregationResponse
        {
            Aggregations = model.Aggregations.Select(e => e.ToDto()).ToArray()
        };
    }

    public static OrdersResponse ToDto(this Grpc.Orders.GetCustomerOrdersResponse model)
    {
        return new OrdersResponse
        {
            Orders = model.Orders.Select(o => o.ToDto()).ToArray()
        };
    }
}