using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery;

public interface IServiceDiscoveryRepository
{
    public DbEndpoint GetEndpointByBucket(int bucket);
    
    public Task<DbEndpoint[]> GetEndpoints(CancellationToken token);

    public IAsyncEnumerable<DbEndpoint[]> GetEndpointsStream(CancellationToken token);
}