using Grpc.Core;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Grpc.ServiceDiscovery;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Grpc;

public class ServiceDiscoveryRepository : IServiceDiscoveryRepository
{
    private readonly SdService.SdServiceClient _sdServiceClient;
    private readonly DbOptions _options;

    public ServiceDiscoveryRepository(SdService.SdServiceClient sdServiceClient, IOptions<DbOptions> options)
    {
        _sdServiceClient = sdServiceClient;
        _options = options.Value;
    }

    public DbEndpointDto GetEndpointByBucket(int bucket)
    {
        throw new NotImplementedException();
    }

    public async Task<DbEndpointDto[]> GetEndpoints(CancellationToken token)
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

    public async IAsyncEnumerable<DbEndpointDto[]> GetEndpointsStream(CancellationToken token)
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

    private IEnumerable<DbEndpointDto> GetEndpoints(DbResourcesResponse response) =>
        response.Replicas.SelectMany(replica =>
            replica.Buckets.Select(b =>
                new DbEndpointDto
                {
                    HostAndPort = $"{replica.Host}:{replica.Port}",
                    DbReplica = ToDbReplica(replica.Type),
                    Bucket = b
                }));

    private DbReplicaTypeDto ToDbReplica(Replica.Types.ReplicaType replicaType) =>
        replicaType switch
        {
            Replica.Types.ReplicaType.Master => DbReplicaTypeDto.Master,
            Replica.Types.ReplicaType.Sync => DbReplicaTypeDto.Sync,
            Replica.Types.ReplicaType.Async => DbReplicaTypeDto.Async,
            _ => throw new ArgumentOutOfRangeException(nameof(replicaType), replicaType, null)
        };
}