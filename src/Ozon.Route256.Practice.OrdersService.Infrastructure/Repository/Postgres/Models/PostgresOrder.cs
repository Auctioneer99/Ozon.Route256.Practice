namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Models;

internal sealed record PostgresOrder(
    long Id, 
    int Count, 
    decimal TotalSum, 
    decimal TotalWeight, 
    int Type, 
    int State, 
    long RegionFromId, 
    long CustomerId, 
    DateTime CreatedAt);