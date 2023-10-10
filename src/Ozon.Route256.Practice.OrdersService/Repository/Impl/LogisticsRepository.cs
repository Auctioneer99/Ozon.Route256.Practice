using Ozon.Route256.Practice.OrdersService.Repository.Dto;
using Ozon.Route256.Practice.OrdersService.Services.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl;

public sealed class LogisticsRepository : ILogisticsRepository
{
    private readonly Grpc.LogisticsSimulator.LogisticsSimulatorService.LogisticsSimulatorServiceClient _logisticsSimulatorServiceClient;

    public LogisticsRepository(Grpc.LogisticsSimulator.LogisticsSimulatorService.LogisticsSimulatorServiceClient logisticsSimulatorServiceClient)
    {
        _logisticsSimulatorServiceClient = logisticsSimulatorServiceClient;
    }

    public async Task<CancelResultDto> CancelOrder(long id, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var result = await _logisticsSimulatorServiceClient.OrderCancelAsync(new Grpc.LogisticsSimulator.Order() { Id = id }, cancellationToken: token);

        return result.ToDto();
    }
}