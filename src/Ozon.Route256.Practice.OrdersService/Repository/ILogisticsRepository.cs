using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public interface ILogisticsRepository
{
    public Task<CancelResultDto> CancelOrder(long id, CancellationToken token);
}