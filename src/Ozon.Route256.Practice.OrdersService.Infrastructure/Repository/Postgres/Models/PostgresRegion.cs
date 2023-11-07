namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Models;

internal sealed record PostgresRegion(
    long Id, 
    string Name, 
    decimal Latitude, 
    decimal Longitude);