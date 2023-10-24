using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public interface IShardsStore
{
    int BucketsCount { get; }
    
    void UpdateEndpointsAsync(IReadOnlyCollection<DbEndpointDto> dbEndpoints);

    DbEndpointDto GetEndpointByBucket(int bucketId);
}