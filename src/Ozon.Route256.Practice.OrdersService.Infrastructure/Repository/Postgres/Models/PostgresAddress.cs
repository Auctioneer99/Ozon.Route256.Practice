namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Models;

internal sealed record PostgresAddress(
    long Id, 
    long RegionId, 
    long OrderId, 
    long CustomerId, 
    string City,
    string Street, 
    string Building, 
    string Apartment, 
    decimal Latitude, 
    decimal Longitude);