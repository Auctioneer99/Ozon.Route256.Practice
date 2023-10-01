using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.GatewayService.Models;

namespace Ozon.Route256.Practice.GatewayService.Services.Mapper;

public static class MappingExtensions
{
    public static OrdersService.Order.Types.OrderType Map(this OrderType type)
    {
        return (OrdersService.Order.Types.OrderType)type;
    }

    public static OrdersService.Order.Types.OrderState Map(this OrderState type)
    {
        return (OrdersService.Order.Types.OrderState)type;
    }
    
    public static OrderType Map(this OrdersService.Order.Types.OrderType type)
    {
        return (OrderType)type;
    }

    public static OrderState Map(this OrdersService.Order.Types.OrderState type)
    {
        return (OrderState)type;
    }
    
    public static OrdersService.Order FromDto(this Order dto)
    {
        return new OrdersService.Order
        {
            Id = dto.Id,
            Count = dto.Count,
            TotalSum = dto.TotalSum,
            TotalWeight = dto.TotalWeight,
            Type = dto.Type.Map(),
            CreatedAt = dto.CreatedAt.ToUniversalTime().ToTimestamp(),
            RegionFrom = dto.RegionFrom,
            State = dto.State.Map(),
            ClientName = dto.ClientName,
            OrderAddress = dto.OrderAddress.FromDtoToOrder(),
            Phone = dto.Phone
        };
    }

    public static Customers.Address FromDtoToCustomer(this Address dto)
    {
        return new Customers.Address
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

    public static OrdersService.Order.Types.Address FromDtoToOrder(this Address dto)
    {
        return new OrdersService.Order.Types.Address
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

    public static OrdersService.PagingRequest FromDto(this PagingRequest dto)
    {
        return new OrdersService.PagingRequest
        {
            SkipCount = dto.SkipCount,
            TakeCount = dto.TakeCount
        };
    }

    public static OrdersService.CancelRequest FromDto(this CancelRequest dto)
    {
        return new OrdersService.CancelRequest
        {
            Id = dto.Id
        };
    }

    public static OrdersService.Order.Types.OrderType FromDto(this OrderType type)
    {
        return (OrdersService.Order.Types.OrderType)type;
    }

    public static OrdersService.SortType FromDto(this SortType type)
    {
        return (OrdersService.SortType)type;
    }

    public static OrdersService.Order.Types.SortField FromDto(this OrderFilterField type)
    {
        return (OrdersService.Order.Types.SortField)type;
    }
    
    public static OrdersService.GetOrdersRequest FromDto(this OrdersRequest dto)
    {
        return new OrdersService.GetOrdersRequest
        {
            OrderTypeFilter = dto.OrderTypeFilter.FromDto(),
            Page = dto.Page.FromDto(),
            Sort = dto.Sort.FromDto(),
            SortField = dto.SortField.FromDto(),
            RegionFilter = { dto.RegionFilter }
        };
    }

    public static OrdersService.GetOrdersAggregationRequest FromDto(this OrdersAggregationRequest dto)
    {
        return new OrdersService.GetOrdersAggregationRequest
        {
            FromDate = dto.FromDate.ToUniversalTime().ToTimestamp(),
            Regions = { dto.Regions }
        };
    }

    public static OrdersService.GetClientOrdersRequest FromDto(this ClientOrdersRequest dto)
    {
        return new OrdersService.GetClientOrdersRequest
        {
            ClientId = dto.ClientId,
            From = dto.From.ToUniversalTime().ToTimestamp(),
            Page = dto.Page.FromDto()
        };
    }

    public static Address ToDto(this Customers.Address model)
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

    public static Address ToDto(this OrdersService.Order.Types.Address model)
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
    
    public static Customer ToDto(this Customers.Customer model)
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
    
    public static CustomersResponse ToDto(this Customers.GetCustomersResponse model)
    {
        return new CustomersResponse
        {
            Customers = model.Customers.Select(c => c.ToDto()).ToArray()
        };
    }

    public static CancelResponse ToDto(this OrdersService.CancelResponse model)
    {
        return new CancelResponse
        {
            IsSuccess = model.IsSuccess,
            Error = model.Error
        };
    }

    public static StatusResponse ToDto(this OrdersService.GetStatusByIdResponse model)
    {
        return new StatusResponse
        {
            Status = model.State.ToString()
        };
    }

    public static RegionsResponse ToDto(this OrdersService.GetRegionsResponse model)
    {
        return new RegionsResponse
        {
            Regions = model.Regions.ToArray()
        };
    }
    
    public static Order ToDto(this OrdersService.Order model)
    {
        return new Order
        {
            Id = model.Id,
            Count = model.Count,
            TotalSum = model.TotalSum,
            TotalWeight = model.TotalWeight,
            Type = model.Type.Map(),
            CreatedAt = model.CreatedAt.ToDateTime(),
            RegionFrom = model.RegionFrom,
            State = model.State.Map(),
            ClientName = model.ClientName,
            OrderAddress = model.OrderAddress.ToDto(),
            Phone = model.Phone
        };
    }
    
    public static OrdersResponse ToDto(this OrdersService.GetOrdersResponse model)
    {
        return new OrdersResponse
        {
            Orders = model.Orders.Select(o => o.ToDto()).ToArray()
        };
    }

    public static OrdersAggregationResponse.OrdersAggregationResponseEntry ToDto(this OrdersService.GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry model)
    {
        return new OrdersAggregationResponse.OrdersAggregationResponseEntry
        {
            Region = model.Region,
            OrdersCount = model.OrdersCount,
            TotalOrdersSum = model.TotalOrdersSum,
            TotalOrdersWeight = model.TotalOrdersWeight,
            UniqueClientsCount = model.UniqueClientsCount
        };
    }
    
    public static OrdersAggregationResponse ToDto(this OrdersService.GetOrdersAggregationResponse model)
    {
        return new OrdersAggregationResponse
        {
            Aggregations = model.Aggregations.Select(e => e.ToDto()).ToArray()
        };
    }

    public static OrdersResponse ToDto(this OrdersService.GetClientOrdersResponse model)
    {
        return new OrdersResponse
        {
            Orders = model.Orders.Select(o => o.ToDto()).ToArray()
        };
    }
}