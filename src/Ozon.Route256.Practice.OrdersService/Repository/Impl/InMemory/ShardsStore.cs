using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.InMemory;

public class ShardsStore : IShardsStore
{
    private DbEndpointDto[] _endpoints = Array.Empty<DbEndpointDto>();
    
    public int BucketsCount { get; private set; }

    public DbEndpointDto GetEndpointByBucket(int bucketId)
    {
        var result = _endpoints.FirstOrDefault(x => x.Bucket == bucketId);

        if (result is null)
        {
            throw new ArgumentOutOfRangeException($"There is no info about bucket {bucketId}");
        }

        return result;
    }
    
    public void UpdateEndpointsAsync(IReadOnlyCollection<DbEndpointDto> dbEndpoints)
    {
        if (BucketsCount != default && BucketsCount != dbEndpoints.Count)
        {
            throw new InvalidOperationException("Buckets count have been changed");
        }
        
        BucketsCount = dbEndpoints.Count;
        _endpoints   = dbEndpoints.ToArray();
    }
}