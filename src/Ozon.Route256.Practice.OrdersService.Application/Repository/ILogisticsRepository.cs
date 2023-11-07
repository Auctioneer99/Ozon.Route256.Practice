using Ozon.Route256.Practice.OrdersService.Application.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Repository;

public interface ILogisticsRepository
{
    public Task<CancelOrderResponse> CancelOrder(long id, CancellationToken token);
}