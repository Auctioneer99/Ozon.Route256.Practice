using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Grpc.Orders;
using Order = Ozon.Route256.Practice.OrdersService.Grpc.Orders.Order;

namespace Ozon.Route256.Practice.OrdersService.Mapping;

internal static class MappingExtensions
{
    public static Order.Types.OrderState ToHost(this OrderState state)
    {
        return state switch
        {
            OrderState.UndefinedState => throw new NotSupportedException(nameof(state)),
            
            OrderState.Created => Order.Types.OrderState.Created,
            OrderState.SentToCustomer => Order.Types.OrderState.SentToCustomer,
            OrderState.Delivered => Order.Types.OrderState.Delivered,
            OrderState.Lost => Order.Types.OrderState.Lost,
            OrderState.Cancelled => Order.Types.OrderState.Cancelled,
            
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }
    
    public static Order.Types.OrderType ToHost(this OrderType state)
    {
        return state switch
        {
            OrderType.Undefined => throw new NotSupportedException(nameof(state)),
            
            OrderType.Web => Order.Types.OrderType.Web,
            OrderType.Mobile => Order.Types.OrderType.Mobile,
            OrderType.Api => Order.Types.OrderType.Api,
            
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };
    }

    public static CancelResponse ToHost(this CancelOrderResponse result)
    {
        return new CancelResponse()
        {
            IsSuccess = result.Success,
            Error = result.Error ?? ""
        };
    }

    public static Order ToHost(this OrderAggregate aggregate)
    {
        return new Order
        {
            Id = aggregate.Order.Id,
            Count = (int)aggregate.Order.Count,
            TotalSum = (double)aggregate.Order.TotalSum,
            TotalWeight = (double)aggregate.Order.TotalWeight,
            Type = aggregate.Order.Type.ToHost(),
            State = aggregate.Order.State.ToHost(),
            CreatedAt = aggregate.Order.CreatedAt.ToUniversalTime().ToTimestamp(),
            RegionFrom = aggregate.Region.Name,
            OrderAddress = aggregate.Address.ToHost(aggregate.Region),
            CustomerName = $"{aggregate.Customer.FirstName} {aggregate.Customer.LastName}",
            Phone = aggregate.Customer.MobileNumber
        };
    }

    public static Order.Types.Address ToHost(this Address address, Region region)
    {
        return new Order.Types.Address
        {
            Region = region.Name,
            City = address.City,
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            Latitude = (double)address.Latitude,
            Longitude = (double)address.Longitude
        };
    }

    public static GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry ToHost(this OrderAggregation aggregation)
    {
        return new GetOrdersAggregationResponse.Types.GetOrdersAggregationResponseEntry()
        {
            Region = aggregation.Region,
            OrdersCount = (int)aggregation.OrdersCount,
            TotalOrdersSum = (int)aggregation.TotalOrdersSum,
            TotalOrdersWeight = (double)aggregation.TotalOrdersWeight,
            UniqueCustomersCount = (int)aggregation.UniqueCustomersCount
        };
    }

    
    public static OrderField ToApplication(this Order.Types.SortField field)
    {
        return field switch
        {
            Order.Types.SortField.NoneField => OrderField.NoneField,
            Order.Types.SortField.Id => OrderField.Id,
            Order.Types.SortField.Count => OrderField.Count,
            Order.Types.SortField.TotalSum => OrderField.TotalSum,
            Order.Types.SortField.TotalWeight => OrderField.TotalWeight,
            Order.Types.SortField.OrderType => OrderField.OrderType,
            Order.Types.SortField.CreatedAt => OrderField.CreatedAt,
            Order.Types.SortField.OrderState => OrderField.OrderState,
            
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
        };
    }

    public static OrderType ToApplication(this Order.Types.OrderType field)
    {
        return field switch
        {
            Order.Types.OrderType.UndefinedType => throw new NotSupportedException(nameof(field)),
            
            Order.Types.OrderType.Web => OrderType.Web,
            Order.Types.OrderType.Mobile => OrderType.Mobile,
            Order.Types.OrderType.Api => OrderType.Api,
            
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
        };
    }

    public static SortingType ToApplication(this SortType field)
    {
        return field switch
        {
            SortType.None => SortingType.None,
            SortType.Ascending => SortingType.Ascending,
            SortType.Descending => SortingType.Descending,
            
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, null)
        };
    }

    public static OrdersRequest ToApplication(this GetOrdersRequest request)
    {
        return new OrdersRequest(
            SortingType: request.Sort.ToApplication(),
            OrderField: request.SortField.ToApplication(),
            OrderType: request.OrderTypeFilter.ToApplication(),
            TakeCount: request.Page.TakeCount,
            SkipCount: request.Page.SkipCount,
            Regions: request.RegionFilter.ToArray(),
            From: new DateTime()
        );
    }

    public static OrdersAggregationRequest ToApplication(this GetOrdersAggregationRequest request)
    {
        return new OrdersAggregationRequest(
            Regions: request.Regions.ToArray(),
            FromDate: request.FromDate.ToDateTime());
    }

    public static CustomerOrdersRequest ToApplication(this GetCustomerOrdersRequest request)
    {
        return new CustomerOrdersRequest(
            CustomerId: request.CustomerId,
            From: request.From.ToDateTime(),
            SkipCount: request.Page.SkipCount,
            TakeCount: request.Page.TakeCount);
    }
}