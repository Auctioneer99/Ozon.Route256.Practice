using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.InMemory.Dal;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.InMemory;

public sealed class RegionRepository : IRegionRepository
{
    private readonly InMemoryStorage _storage;

    public RegionRepository(InMemoryStorage storage)
    {
        _storage = storage;
    }

    public Task<Region?> FindById(long id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Region?>(token);
        }

        return _storage.Regions
            .TryGetValue(id, out var region)
            ? Task.FromResult<Region?>(region)
            : Task.FromResult<Region?>(null);
    }

    public Task<Region?> FindByName(string name, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Region?>(token);
        }

        var region = _storage
            .Regions
            .Values
            .FirstOrDefault(r => r.Name == name);

        return Task.FromResult(region);
    }

    public Task<Region[]> FindManyById(IEnumerable<long> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Region[]>(token);
        }

        var regions = FindDto(ids, token)
            .ToArray();
        return Task.FromResult(regions);
    }

    public Task<Region> GetById(long id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Region>(token);
        }

        return _storage.Regions.TryGetValue(id, out var region)
            ? Task.FromResult(region)
            : throw new NotFoundException($"Регион с ID = {id} не найден");
    }

    public Task<Region[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Region[]>(token);
        }
        
        var regions = new List<Region>();
        
        var notFoundRegions = new HashSet<long>();
        
        foreach (var id in ids)
        {
            if (token.IsCancellationRequested)
            {
                return Task.FromCanceled<Region[]>(token);
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

    public async Task<Region[]> GetManyByName(IEnumerable<string> regionNames, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        
        var regions = new List<Region>();
        
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

    private IEnumerable<Region> FindDto(IEnumerable<long> ids, CancellationToken token)
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

    public Task<Region[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<Region[]>(token);
        }

        return Task.FromResult(_storage.Regions.Values.ToArray());
    }
}