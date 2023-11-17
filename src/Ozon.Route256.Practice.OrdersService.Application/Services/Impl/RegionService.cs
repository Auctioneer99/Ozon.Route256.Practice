using Microsoft.Extensions.Logging;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Services.Impl;

public sealed class RegionService : IRegionService
{
    private readonly ILogger<RegionService> _logger;
    
    private readonly IRegionRepository _regionRepository;

    public RegionService(
        ILogger<RegionService> logger,
        IRegionRepository regionRepository)
    {
        _logger = logger;
        _regionRepository = regionRepository;
    }

    public async Task<Region[]> GetRegions(CancellationToken token)
    {
        _logger.LogDebug("Get regions request");
        
        var response = await _regionRepository.GetAll(token);
        
        _logger.LogDebug("Get regions response {@Response}", response.AsEnumerable());
        
        return response;
    }
}