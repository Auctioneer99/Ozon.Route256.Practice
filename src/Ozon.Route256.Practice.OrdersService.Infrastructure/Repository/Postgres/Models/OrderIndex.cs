namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Models;

internal sealed record OrderIndex(
    long OrderId, 
    int Shard);