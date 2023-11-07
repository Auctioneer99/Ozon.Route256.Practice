namespace Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Dto;

public sealed record DbEndpoint(
    string HostAndPort,
    DbReplicaType DbReplica,
    int Bucket);