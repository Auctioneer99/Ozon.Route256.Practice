using System.Text.Json;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Grpc;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Redis.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Redis.Models;
using StackExchange.Redis;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Redis;

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
    
    public async Task<Customer> GetById(long id, CancellationToken token)
    {
        var value = await _database
            .StringGetAsync(GetKey(id))
            .WaitAsync(token);

        var model = ConvertToDto(value);

        if (model != null)
        {
            return model.ToDomain();
        }

        return await UpdateValue(id, token);
    }

    public async Task<Customer[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
    {
        var valueTasks = ids
            .Select(id => GetById(id, token))
            .ToList();

        var values = await Task.WhenAll(valueTasks);

        return values;
    }

    private async Task<Customer> UpdateValue(long id, CancellationToken token)
    {
        var model = await _customerRepository.GetById(id, token);

        var dto = model.ToRedis();
        await _database
            .StringSetAsync(GetKey(model.Id), ConvertToRedisValue(dto), _ttl)
            .WaitAsync(token);

        return model;
    }

    private string GetKey(long id)
    {
        return $"customer:{id}";
    }

    private RedisCustomer? ConvertToDto(RedisValue value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var model = JsonSerializer.Deserialize<RedisCustomer?>(value.ToString());

        return model;
    }

    private RedisValue ConvertToRedisValue(RedisCustomer dto)
    {
        return JsonSerializer.Serialize(dto);
    }
}