using Dapper;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public class PostgresRegionRepository : BaseShardRepository, IRegionRepository
{
    private readonly IShardsStore _store;
    private readonly Random _random;
    
    private const string Fields = $"id as {nameof(RegionDto.Id)}, name as {nameof(RegionDto.Name)}, latitude as {nameof(RegionDto.Latitude)}, longitude as {nameof(RegionDto.Longitude)}";
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
    
    public async Task<RegionDto?> FindById(long id, CancellationToken token)
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
        
        return await connection.QueryFirstAsync<RegionDto?>(cmd);
    }

    public async Task<RegionDto?> FindByName(string name, CancellationToken token)
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
        
        return await connection.QueryFirstOrDefaultAsync<RegionDto?>(cmd);
    }

    public async Task<RegionDto[]> FindManyById(IEnumerable<long> ids, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = any(:ids);";

        await using var connection = GetConnectionByShardKey(_random.Next(_store.BucketsCount));
        
        var param = new DynamicParameters();
        param.Add("ids", ids);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        return (await connection.QueryAsync<RegionDto>(cmd)).ToArray();
    }

    public async Task<RegionDto[]> FindManyByName(IEnumerable<string> names, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where name = any(:names);";

        await using var connection = GetConnectionByShardKey(_random.Next(_store.BucketsCount));
        
        var param = new DynamicParameters();
        param.Add("names", names);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        return (await connection.QueryAsync<RegionDto>(cmd)).ToArray();
    }

    public async Task<RegionDto> GetById(long id, CancellationToken token)
    {
        var result = await FindById(id, token);

        if (result == null)
        {
            throw new NotFoundException($"Регион с ID = {id} не найден");
        }

        return result;
    }

    public async Task<RegionDto[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
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

    public async Task<RegionDto[]> GetManyByName(IEnumerable<string> regionNames, CancellationToken token)
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

    public async Task<RegionDto[]> GetAll(CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}";

        await using var connection = GetConnectionByShardKey(_random.Next(_store.BucketsCount));
        
        var cmd = new CommandDefinition(
            sql,
            cancellationToken: token);
        
        return (await connection.QueryAsync<RegionDto>(cmd)).ToArray();
    }
}