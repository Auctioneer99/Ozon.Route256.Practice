using System.Text.Json;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;
using Ozon.Route256.Practice.OrdersService.Repository.Impl.Grpc;
using StackExchange.Redis;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Redis;

public sealed class RedisCustomerRepository : ICustomerRepository
{
    private readonly IDatabase _database;
    private readonly IServer _server;

    private readonly CustomerRepository _customerRepository;

    private readonly TimeSpan _ttl;

    public RedisCustomerRepository(IRedisDatabaseFactory factory, CustomerRepository repository, TimeSpan ttl)
    {
        _database = factory.GetDatabase();
        _server = factory.GetServer();

        _customerRepository = repository;
        _ttl = ttl;
    }
    
    public async Task<CustomerDto> GetById(long id, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var value = await _database
            .StringGetAsync(GetKey(id))
            .WaitAsync(token);

        var model = ConvertToDto(value);

        if (model != null)
        {
            return model;
        }

        return await UpdateValue(id, token);
    }

    public async Task<CustomerDto[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        var valueTasks = ids
            .Select(id => GetById(id, token))
            .ToList();

        var values = await Task.WhenAll(valueTasks);

        return values;
    }

    private async Task<CustomerDto> UpdateValue(long id, CancellationToken token)
    {
        var model = await _customerRepository.GetById(id, token);

        await _database
            .StringSetAsync(GetKey(model.Id), ConvertToRedisValue(model), _ttl)
            .WaitAsync(token);

        return model;
    }

    private string GetKey(long id)
    {
        return $"customer:{id}";
    }

    private CustomerDto? ConvertToDto(RedisValue value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var model = JsonSerializer.Deserialize<CustomerDto?>(value.ToString());

        return model;
    }

    private RedisValue ConvertToRedisValue(CustomerDto dto)
    {
        return JsonSerializer.Serialize(dto);
    }
}