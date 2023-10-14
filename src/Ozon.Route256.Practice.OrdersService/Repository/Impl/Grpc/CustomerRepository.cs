using Ozon.Route256.Practice.OrdersService.Grpc.Customers;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;
using Ozon.Route256.Practice.OrdersService.Services.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Grpc;

public sealed class CustomerRepository : ICustomerRepository
{
    private readonly Customers.CustomersClient _customersClient;

    public CustomerRepository(Customers.CustomersClient customersClient)
    {
        _customersClient = customersClient;
    }

    public async Task<CustomerDto> GetById(long id, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var response = await _customersClient.GetCustomerAsync(new GetCustomerByIdRequest() { Id = (int)id }, cancellationToken: token);

        return response.ToDto();
    }

    public async Task<CustomerDto[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var result = new List<CustomerDto>(ids.Count());
        foreach (var id in ids)
        {
            var customer = await GetById(id, token);
            result.Add(customer);
        }

        return result.ToArray();
    }
}