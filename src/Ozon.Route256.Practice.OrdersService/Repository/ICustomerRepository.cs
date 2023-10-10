using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public interface ICustomerRepository
{
    public Task<CustomerDto> GetById(long id, CancellationToken token);

    public Task<CustomerDto[]> GetManyById(IEnumerable<long> ids, CancellationToken token);
}