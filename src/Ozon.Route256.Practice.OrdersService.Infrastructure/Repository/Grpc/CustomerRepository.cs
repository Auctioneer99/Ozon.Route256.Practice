using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Grpc.Customers;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;
using Customer = Ozon.Route256.Practice.OrdersService.Domain.Models.Customer;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Grpc;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly Customers.CustomersClient _customersClient;

    public CustomerRepository(Customers.CustomersClient customersClient)
    {
        _customersClient = customersClient;
    }

    public async Task<Customer> GetById(long id, CancellationToken token)
    {
        var response = await _customersClient.GetCustomerAsync(new GetCustomerByIdRequest() { Id = (int)id }, cancellationToken: token);

        return response.ToDomain();
    }

    public async Task<Customer[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
    {
        var result = new List<Customer>(ids.Count());
        foreach (var id in ids)
        {
            var customer = await GetById(id, token);
            result.Add(customer);
        }

        return result.ToArray();
    }
}