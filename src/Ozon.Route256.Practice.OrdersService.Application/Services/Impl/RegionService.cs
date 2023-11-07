using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Services.Impl;

public sealed class RegionService : IRegionService
{
    private readonly IRegionRepository _regionRepository;

    public RegionService(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    public async Task<Region[]> GetRegions(CancellationToken token)
    {
        return await _regionRepository.GetAll(token);
    }
}