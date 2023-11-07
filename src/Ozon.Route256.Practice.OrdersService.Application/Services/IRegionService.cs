using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Services;

public interface IRegionService
{
    public Task<Region[]> GetRegions(CancellationToken token);
}