using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl;

public sealed class RegionRepository : IRegionRepository
{
    private readonly InMemoryStorage _storage;

    public RegionRepository(InMemoryStorage storage)
    {
        _storage = storage;
    }

    public Task<RegionDto?> FindById(long id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<RegionDto?>(token);
        }

        return _storage.Regions
            .TryGetValue(id, out var region)
            ? Task.FromResult<RegionDto?>(region)
            : Task.FromResult<RegionDto?>(null);
    }

    public Task<RegionDto?> FindByName(string name, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<RegionDto?>(token);
        }

        var region = _storage
            .Regions
            .Values
            .FirstOrDefault(r => r.Name == name);

        return Task.FromResult(region);
    }

    public Task<RegionDto[]> FindManyById(IEnumerable<long> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<RegionDto[]>(token);
        }

        var regions = FindDto(ids, token)
            .ToArray();
        return Task.FromResult(regions);
    }

    public Task<RegionDto[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<RegionDto[]>(token);
        }
        
        var regions = new List<RegionDto>();
        
        var notFoundRegions = new HashSet<long>();
        
        foreach (var id in ids)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled<RegionDto[]>(token);
            }
            
            if (_storage.Regions.TryGetValue(id, out var region))
            {
                regions.Add(region);
            }
            else
            {
                notFoundRegions.Add(id);
            }
        }

        if (notFoundRegions.Count > 0)
        {
            throw new NotExistsException($"Регионы с ID ({string.Join(", ", notFoundRegions)}) не существуют");
        }

        return Task.FromResult(regions.ToArray());
    }

    public async Task<RegionDto[]> GetManyByName(IEnumerable<string> regionNames, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var regions = new List<RegionDto>();
        
        var notFoundRegions = new HashSet<string>();
        
        foreach (var regionName in regionNames)
        {
            token.ThrowIfCancellationRequested();
            
            var region = await FindByName(regionName, token);

            if (region is null)
            {
                notFoundRegions.Add(regionName);
            }
            else
            {
                regions.Add(region);
            }
        }

        if (notFoundRegions.Count > 0)
        {
            throw new NotExistsException($"Регионы ({string.Join(", ", notFoundRegions)}) не существуют");
        }

        return regions.ToArray();
    }

    private IEnumerable<RegionDto> FindDto(IEnumerable<long> ids, CancellationToken token)
    {
        foreach (var id in ids)
        {
            token.ThrowIfCancellationRequested();

            if (_storage.Regions.TryGetValue(id, out var region))
            {
                yield return region;
            }
        }
    }

    public Task<RegionDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<RegionDto[]>(token);
        }

        return Task.FromResult(_storage.Regions.Values.ToArray());
    }
}