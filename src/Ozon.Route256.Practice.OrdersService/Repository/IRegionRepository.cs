using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public interface IRegionRepository
{
    public Task<RegionDto?> FindById(long id, CancellationToken token);
    
    public Task<RegionDto?> FindByName(string name, CancellationToken token);

    public Task<RegionDto[]> FindManyById(IEnumerable<long> ids, CancellationToken token);

    public Task<RegionDto> GetById(long id, CancellationToken token);
    
    public Task<RegionDto[]> GetManyById(IEnumerable<long> ids, CancellationToken token);
    
    public Task<RegionDto[]> GetManyByName(IEnumerable<string> regionNames, CancellationToken token);
    
    public Task<RegionDto[]> GetAll(CancellationToken token);
}