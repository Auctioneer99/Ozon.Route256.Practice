using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Impl;

public sealed class ShardsStore : IShardsStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();
    
    public int BucketsCount { get; private set; }

    public DbEndpoint GetEndpointByBucket(int bucketId)
    {
        var result = _endpoints.FirstOrDefault(x => x.Bucket == bucketId);

        if (result is null)
        {
            throw new ArgumentOutOfRangeException($"There is no info about bucket {bucketId}");
        }

        return result;
    }
    
    public void UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        if (BucketsCount != default && BucketsCount != dbEndpoints.Count)
        {
            throw new InvalidOperationException("Buckets count have been changed");
        }
        
        BucketsCount = dbEndpoints.Count;
        _endpoints   = dbEndpoints.ToArray();
    }
}