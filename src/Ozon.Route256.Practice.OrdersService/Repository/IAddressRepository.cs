using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public interface IAddressRepository
{
    public Task<AddressDto?> FindById(int id, CancellationToken token);

    public Task<AddressDto?> FindByCoordinates(double latitude, double longitude, CancellationToken token);
    
    public Task<AddressDto[]> FindManyByOrderId(IEnumerable<long> ids, CancellationToken token);
    
    public Task<AddressDto[]> GetManyByOrderId(IEnumerable<long> ids, CancellationToken token);

    public Task<AddressDto[]> GetAll(CancellationToken token);

    public Task<AddressDto> Add(AddressDto dto, CancellationToken token);
}