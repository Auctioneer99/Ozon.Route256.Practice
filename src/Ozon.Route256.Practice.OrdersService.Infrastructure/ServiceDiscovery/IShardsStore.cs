using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery;

public interface IShardsStore
{
    int BucketsCount { get; }
    
    void UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);

    DbEndpoint GetEndpointByBucket(int bucketId);
}