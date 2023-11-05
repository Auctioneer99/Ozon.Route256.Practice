using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Repository;

public interface IRegionRepository
{
    public Task<Region?> FindById(long id, CancellationToken token);
    
    public Task<Region?> FindByName(string name, CancellationToken token);

    public Task<Region[]> FindManyById(IEnumerable<long> ids, CancellationToken token);

    public Task<Region> GetById(long id, CancellationToken token);
    
    public Task<Region[]> GetManyById(IEnumerable<long> ids, CancellationToken token);
    
    public Task<Region[]> GetManyByName(IEnumerable<string> regionNames, CancellationToken token);
    
    public Task<Region[]> GetAll(CancellationToken token);
}