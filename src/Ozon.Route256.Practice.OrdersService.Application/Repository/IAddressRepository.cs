using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Repository;

public interface IAddressRepository
{
    public Task<Address?> FindById(int id, CancellationToken token);

    public Task<Address?> FindByCoordinates(double latitude, double longitude, CancellationToken token);
    
    public Task<Address[]> FindManyByOrderId(IEnumerable<long> ids, CancellationToken token);
    
    public Task<Address[]> GetManyByOrderId(IEnumerable<long> ids, CancellationToken token);

    public Task<Address[]> GetAll(CancellationToken token);

    public Task<Address> Add(Address address, CancellationToken token);
}