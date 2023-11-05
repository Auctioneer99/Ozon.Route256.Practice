using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Redis.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;

internal static class RepositoryMappingExtensions
{
    public static Customer ToDomain(this Grpc.Customers.Customer customer)
    {
        return new Customer(
            id: customer.Id,
            firstName: customer.FirstName,
            lastName: customer.LastName,
            phone: customer.MobileNumber,
            email: customer.Email);
    }

    public static CancelOrderResponse ToApplication(this Grpc.LogisticsSimulator.CancelResult result)
    {
        return new CancelOrderResponse(
            Success: result.Success,
            Error: result.Error);
    }

    public static RedisCustomer ToRedis(this Customer customer)
    {
        return new RedisCustomer(
            Id: customer.Id,
            FirstName: customer.FirstName,
            LastName: customer.LastName,
            MobileNumber: customer.MobileNumber,
            Email: customer.Email);
    }

    public static Customer ToDomain(this RedisCustomer customer)
    {
        return new Customer(
            id: customer.Id,
            firstName: customer.FirstName,
            lastName: customer.LastName,
            phone: customer.MobileNumber,
            email: customer.Email);
    }

    public static PostgresAddress ToPostgres(this Address address)
    {
        return new PostgresAddress(
            Id: address.Id,
            RegionId: address.RegionId,
            OrderId: address.OrderId,
            CustomerId: address.CustomerId,
            City: address.City,
            Street: address.Street,
            Building: address.Building,
            Apartment: address.Apartment,
            Latitude: address.Latitude,
            Longitude: address.Longitude);
    }

    public static Address ToDomain(this PostgresAddress address)
    {
        return new Address(
            id: address.Id,
            regionId: address.RegionId,
            orderId: address.OrderId,
            customerId: address.CustomerId,
            city: address.City,
            street: address.Street,
            building: address.Building,
            apartment: address.Apartment,
            latitude: address.Latitude,
            longitude: address.Longitude);
    }

    public static PostgresOrder ToPostgres(this Order order)
    {
        return new PostgresOrder(
            CustomerId: order.CustomerId,
            Id: order.Id,
            Count: (int)order.Count,
            TotalSum: order.TotalSum,
            TotalWeight: order.TotalWeight,
            Type: (int)order.Type,
            State: (int)order.State,
            RegionFromId: order.RegionFromId,
            CreatedAt: order.CreatedAt);
    }

    public static Order ToDomain(this PostgresOrder order)
    {
        return new Order(
            customerId: order.CustomerId,
            id: order.Id,
            count: order.Count,
            totalSum: order.TotalSum,
            totalWeight: order.TotalWeight,
            type: (OrderType)order.Type,
            state: (OrderState)order.State,
            regionFromId: order.RegionFromId,
            createdAt: order.CreatedAt);
    }

    public static PostgresRegion ToPostgres(this Region region)
    {
        return new PostgresRegion(
            Id: region.Id,
            Name: region.Name,
            Latitude: region.Latitude,
            Longitude: region.Longitude);
    }

    public static Region ToDomain(this PostgresRegion region)
    {
        return new Region(
            id: region.Id,
            name: region.Name,
            latitude: region.Latitude,
            longitude: region.Longitude);
    }
}