using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public interface IAddressRepository
{
    public Task<AddressDto?> Find(int id, CancellationToken token);

    public Task<AddressDto[]> FindManyById(IEnumerable<long> ids, CancellationToken token);
    
    public Task<AddressDto[]> GetManyById(IEnumerable<long> ids, CancellationToken token);

    public Task<AddressDto[]> GetAll(CancellationToken token);
}