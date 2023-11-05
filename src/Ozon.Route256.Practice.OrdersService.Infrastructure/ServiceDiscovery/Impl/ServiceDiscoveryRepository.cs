using System.Runtime.CompilerServices;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Grpc.ServiceDiscovery;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Dto;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Impl;

public sealed class ServiceDiscoveryRepository : IServiceDiscoveryRepository
{
    private readonly SdService.SdServiceClient _sdServiceClient;
    private readonly DbOptions _options;

    public ServiceDiscoveryRepository(SdService.SdServiceClient sdServiceClient, IOptions<DbOptions> options)
    {
        _sdServiceClient = sdServiceClient;
        _options = options.Value;
    }

    public DbEndpoint GetEndpointByBucket(int bucket)
    {
        throw new NotImplementedException();
    }

    public async Task<DbEndpoint[]> GetEndpoints(CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var request = new DbResourcesRequest
        {
            ClusterName = _options.ClusterName
        };

        using var stream = _sdServiceClient.DbResources(request, cancellationToken: token);
        
        await stream.ResponseStream.MoveNext(token);
        var response = stream.ResponseStream.Current;
        var endpoints = GetEndpoints(response);

        return endpoints.ToArray();
    }

    public async IAsyncEnumerable<DbEndpoint[]> GetEndpointsStream([EnumeratorCancellation] CancellationToken token)
    {
        var request = new DbResourcesRequest
        {
            ClusterName = _options.ClusterName
        };

        using var stream = _sdServiceClient.DbResources(request, cancellationToken: token);
        
        await foreach (var response in stream.ResponseStream.ReadAllAsync(token))
        {
            yield return GetEndpoints(response).ToArray();
        }
    }

    private IEnumerable<DbEndpoint> GetEndpoints(DbResourcesResponse response) =>
        response.Replicas.SelectMany(replica =>
            replica.Buckets.Select(b =>
                new DbEndpoint(
                    HostAndPort: $"{replica.Host}:{replica.Port}",
                    DbReplica: replica.Type.ToInfrastructure(),
                    Bucket: b)));
}