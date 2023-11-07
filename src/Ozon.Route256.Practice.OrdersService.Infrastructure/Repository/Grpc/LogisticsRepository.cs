using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Grpc.LogisticsSimulator;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Grpc;

public sealed class LogisticsRepository : ILogisticsRepository
{
    private readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;

    public LogisticsRepository(LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient)
    {
        _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
    }

    public async Task<CancelOrderResponse> CancelOrder(long id, CancellationToken token)
    {
        var result = await _logisticsSimulatorServiceClient.OrderCancelAsync(new Order() { Id = id }, cancellationToken: token);

        return result.ToApplication();
    }
}