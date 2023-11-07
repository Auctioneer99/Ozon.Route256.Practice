using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Application.Repository;

public interface ICustomerRepository
{
    public Task<Customer> GetById(long id, CancellationToken token);

    public Task<Customer[]> GetManyById(IEnumerable<long> ids, CancellationToken token);
}