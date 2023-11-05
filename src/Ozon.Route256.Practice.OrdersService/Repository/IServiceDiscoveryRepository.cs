using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public interface IServiceDiscoveryRepository
{
    public DbEndpointDto GetEndpointByBucket(int bucket);
    
    public Task<DbEndpointDto[]> GetEndpoints(CancellationToken token);

    public IAsyncEnumerable<DbEndpointDto[]> GetEndpointsStream(CancellationToken token);
}