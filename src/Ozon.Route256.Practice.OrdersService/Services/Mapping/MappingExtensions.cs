using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.Customers;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Services.Mapping;

public static class MappingExtensions
{
    public static Order.Types.OrderState FromDto(this OrderState state)
    {
        return (Order.Types.OrderState)state;
    }
    
    public static Order.Types.OrderType FromDto(this OrderType state)
    {
        return (Order.Types.OrderType)state;
    }

    public static OrderField ToDto(this Order.Types.SortField field)
    {
        return (OrderField)field;
    }

    public static SortingType ToDto(this SortType field)
    {
        return (SortingType)field;
    }

    public static OrderType ToDto(this Order.Types.OrderType field)
    {
        return (OrderType)field;
    }

    public static Order FromDto(this OrderDto order, RegionDto region, CustomerDto customer, Order.Types.Address address)
    {
        return new Order
        {
            Id = order.Id,
            Count = order.Count,
            TotalSum = order.TotalSum,
            TotalWeight = order.TotalWeight,
            Type = order.Type.FromDto(),
            State = order.State.FromDto(),
            CreatedAt = order.CreatedAt.ToTimestamp(),
            RegionFrom = region.Name,
            OrderAddress = address,
            CustomerName = $"{customer.FirstName} {customer.LastName}",
            Phone = customer.MobileNumber
        };
    }

    public static Order.Types.Address FromDto(this AddressDto address, RegionDto region)
    {
        return new Order.Types.Address
        {
            Region = region.Name,
            City = address.City,
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            Latitude = address.Latitude,
            Longitude = address.Longitude
        };
    }

    public static CustomerDto ToDto(this Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.MobileNumber,
            customer.Email
            );
    }

    public static OrderRequestDto ToDto(this GetOrdersRequest request, IEnumerable<long> regions)
    {
        return new OrderRequestDto(
            request.Sort.ToDto(),
            request.SortField.ToDto(),
            request.Page.TakeCount,
            request.Page.SkipCount,
            request.OrderTypeFilter.ToDto(),
            regions
        );
    }

    public static CancelResultDto ToDto(this CancelResult result)
    {
        return new CancelResultDto(result.Success, result.Error);
    }

    public static CancelResponse FromDto(this CancelResultDto result)
    {
        return new CancelResponse()
        {
            IsSuccess = result.Success,
            Error = result.Error
        };
    }

    public static OrderRequestByCustomerDto ToDto(this GetCustomerOrdersRequest request)
    {
        return new OrderRequestByCustomerDto(
            request.CustomerId,
            request.From.ToDateTime(),
            request.Page.SkipCount,
            request.Page.TakeCount);
    }
}