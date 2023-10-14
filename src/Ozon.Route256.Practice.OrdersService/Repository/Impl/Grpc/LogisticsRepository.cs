using Ozon.Route256.Practice.OrdersService.Grpc.LogisticsSimulator;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;
using Ozon.Route256.Practice.OrdersService.Services.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Grpc;

public sealed class LogisticsRepository : ILogisticsRepository
{
    private readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;

    public LogisticsRepository(LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient)
    {
        _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
    }

    public async Task<CancelResultDto> CancelOrder(long id, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var result = await _logisticsSimulatorServiceClient.OrderCancelAsync(new Order() { Id = id }, cancellationToken: token);

        return result.ToDto();
    }
}