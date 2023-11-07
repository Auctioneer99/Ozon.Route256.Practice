using Dapper;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres;

public class PostgresRegionRepository : BaseShardRepository, IRegionRepository
{
    private readonly IShardsStore _store;
    private readonly Random _random;
    
    private const string Fields = $"id as {nameof(PostgresRegion.Id)}, name as {nameof(PostgresRegion.Name)}, latitude as {nameof(PostgresRegion.Latitude)}, longitude as {nameof(PostgresRegion.Longitude)}";
    private const string Table = $"{Shards.BucketPlaceholder}.regions";
    
    public PostgresRegionRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule,
        IShardsStore store): base(connectionFactory, longShardingRule, stringShardingRule)
    {
        _store = store;
        _random = new Random();
    }
    
    public async Task<Region?> FindById(long id, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id;";

        await using var connection = GetConnectionByShardKey(_random.Next(_store.BucketsCount));
        
        var param = new DynamicParameters();
        param.Add("id", id);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        var dto = await connection.QueryFirstOrDefaultAsync<PostgresRegion?>(cmd);

        return dto?.ToDomain();
    }

    public async Task<Region?> FindByName(string name, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where name = :name;";

        await using var connection = GetConnectionByShardKey(_random.Next(_store.BucketsCount));
        
        var param = new DynamicParameters();
        param.Add("name", name);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        var dto = await connection.QueryFirstOrDefaultAsync<PostgresRegion?>(cmd);

        return dto?.ToDomain();
    }

    public async Task<Region[]> FindManyById(IEnumerable<long> ids, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = any(:ids);";

        await using var connection = GetConnectionByShardKey(_random.Next(_store.BucketsCount));
        
        var param = new DynamicParameters();
        param.Add("ids", ids.ToList());
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);

        var dto = await connection.QueryAsync<PostgresRegion>(cmd);
        
        return dto.Select(r => r.ToDomain()).ToArray();
    }

    public async Task<Region[]> FindManyByName(IEnumerable<string> names, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where name = any(:names);";

        await using var connection = GetConnectionByShardKey(_random.Next(_store.BucketsCount));
        
        var param = new DynamicParameters();
        param.Add("names", names.ToList());
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        var dto = await connection.QueryAsync<PostgresRegion>(cmd);
        
        return dto.Select(r => r.ToDomain()).ToArray();
    }

    public async Task<Region> GetById(long id, CancellationToken token)
    {
        var result = await FindById(id, token);

        if (result == null)
        {
            throw new NotFoundException($"Регион с ID = {id} не найден");
        }

        return result;
    }

    public async Task<Region[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
    {
        var result = await FindManyById(ids, token);

        var notFound = new List<long>();

        foreach (var id in ids)
        {
            if (result.Any(v => v.Id == id) == false)
            {
                notFound.Add(id);
            }
        }

        if (notFound.Count > 0)
        {
            throw new NotFoundException($"Регионы с ID = ({string.Join(", ", notFound)}) не найдены");
        }

        return result;
    }

    public async Task<Region[]> GetManyByName(IEnumerable<string> regionNames, CancellationToken token)
    {
        var result = await FindManyByName(regionNames, token);

        var notFound = new List<string>();

        foreach (var name in regionNames)
        {
            if (result.Any(v => v.Name == name) == false)
            {
                notFound.Add(name);
            }
        }

        if (notFound.Count > 0)
        {
            throw new NotFoundException($"Регионы ({string.Join(", ", notFound)}) не найдены");
        }

        return result;
    }

    public async Task<Region[]> GetAll(CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}";

        await using var connection = GetConnectionByShardKey(_random.Next(_store.BucketsCount));
        
        var cmd = new CommandDefinition(
            sql,
            cancellationToken: token);
        
        var dto = await connection.QueryAsync<PostgresRegion>(cmd);
        
        return dto.Select(r => r.ToDomain()).ToArray();
    }
}