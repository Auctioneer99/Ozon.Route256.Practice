using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery;

namespace Ozon.Route256.Practice.OrdersService.HostedServices;

internal sealed class SdConsumerHostedService : BackgroundService
{
    private readonly IShardsStore _storage;
    private readonly IServiceDiscoveryRepository _serviceDiscoveryRepository;

    public SdConsumerHostedService(IShardsStore storage, IServiceDiscoveryRepository serviceDiscoveryRepository)
    {
        _storage = storage;
        _serviceDiscoveryRepository = serviceDiscoveryRepository;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var stream = _serviceDiscoveryRepository.GetEndpointsStream(cancellationToken);
                
                await foreach (var response in stream.WithCancellation(cancellationToken))
                {
                    _storage.UpdateEndpointsAsync(response);
                }
            }
            catch (RpcException e)
            {
                if (e.StatusCode == StatusCode.Cancelled)
                {
                    await Task.Delay(10000, cancellationToken);
                    continue;
                }

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}