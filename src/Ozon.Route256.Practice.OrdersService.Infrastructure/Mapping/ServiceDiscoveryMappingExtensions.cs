using Ozon.Route256.Practice.OrdersService.Grpc.ServiceDiscovery;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;

internal static class ServiceDiscoveryMappingExtensions
{
    public static DbReplicaType ToInfrastructure(this Replica.Types.ReplicaType replicaType)
    {
        return replicaType switch
        {
            Replica.Types.ReplicaType.Master => DbReplicaType.Master,
            Replica.Types.ReplicaType.Sync => DbReplicaType.Sync,
            Replica.Types.ReplicaType.Async => DbReplicaType.Async,
            
            _ => throw new ArgumentOutOfRangeException(nameof(replicaType), replicaType, null)
        };
    }
}