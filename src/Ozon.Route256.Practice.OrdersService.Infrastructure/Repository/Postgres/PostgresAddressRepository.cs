using Dapper;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres;

public sealed class PostgresAddressRepository : BaseShardRepository, IAddressRepository
{
    private const string Fields = $"id as {nameof(PostgresAddress.Id)}, region_id as {nameof(PostgresAddress.RegionId)}, order_id as {nameof(PostgresAddress.OrderId)}, customer_id as {nameof(PostgresAddress.CustomerId)}, city as {nameof(PostgresAddress.City)}, street as {nameof(PostgresAddress.Street)}, building as {nameof(PostgresAddress.Building)}, apartment as {nameof(PostgresAddress.Apartment)}, latitude as {nameof(PostgresAddress.Latitude)}, longitude as {nameof(PostgresAddress.Longitude)}";
    private const string FieldsForInsert = "region_id, order_id, customer_id, city, street, building, apartment, latitude, longitude";
    private const string Table = $"{Shards.BucketPlaceholder}.addresses";
    
    private const string IndexFields = $"order_id as {nameof(OrderIndex.OrderId)}, shard as {nameof(OrderIndex.Shard)}";
    private const string IndexFieldsForInsert = "order_id, shard";
    private const string IndexTable = $"{Shards.BucketPlaceholder}.orders_id_global_index";
    
    public PostgresAddressRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule): base(connectionFactory, longShardingRule, stringShardingRule)
    {
    }

    public async Task<Address?> FindById(int id, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id;";

        await using var connection = GetConnectionByShardKey(0);

        var param = new DynamicParameters();
        param.Add("id", id);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        var dto = await connection.QueryFirstOrDefaultAsync<PostgresAddress?>(cmd);

        return dto?.ToDomain();
    }

    public async Task<Address?> FindByCoordinates(double latitude, double longitude, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where latitude > :latitude - 0.01 and latitude < :latitude + 0.01
                and longitude > :longitude - 0.01 and longitude < :longitude + 0.01;";

        await using var connection = GetConnectionByShardKey(0);

        var param = new DynamicParameters();
        param.Add("latitude", latitude);
        param.Add("longitude", longitude);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        var dto = await connection.QueryFirstOrDefaultAsync<PostgresAddress?>(cmd);

        return dto?.ToDomain();
    }

    public async Task<Address[]> FindManyByOrderId(IEnumerable<long> ids, CancellationToken token)
    {
        var orderShards = ids.GroupBy(id => _longShardingRule.GetBucket(id));
        var orderIndexes = new List<OrderIndex>();
        
        foreach (var shard in orderShards)
        {
            const string sql = @$"
                select {IndexFields}
                from {IndexTable}
                where order_id = any(:ids);";
            
            await using (var connection = GetConnectionByBucket(shard.Key))
            {
                var param = new DynamicParameters();
                param.Add("ids", shard.Distinct().ToList());
        
                var cmd = new CommandDefinition(
                    sql,
                    param,
                    cancellationToken: token);
        
                orderIndexes.AddRange(await connection.QueryAsync<OrderIndex>(cmd));
            }
        }

        var addressShards = orderIndexes.GroupBy(o => o.Shard);
        var result = new List<Address>();
        
        foreach (var shard in addressShards)
        {
            const string sql = @$"
                select {Fields}
                from {Table}
                where order_id = any(:ids);";
            
            await using (var connection = GetConnectionByBucket(shard.Key))
            {
                var param = new DynamicParameters();
                param.Add("ids", shard.Select(o => o.OrderId).Distinct().ToList());
        
                var cmd = new CommandDefinition(
                    sql,
                    param,
                    cancellationToken: token);

                var dto = await connection.QueryAsync<PostgresAddress>(cmd);
                
                result.AddRange(dto.Select(a => a.ToDomain()));
            }
        }

        return result.ToArray();
    }

    public async Task<Address[]> GetManyByOrderId(IEnumerable<long> ids, CancellationToken token)
    {
        var result = await FindManyByOrderId(ids, token);

        var notFound = new List<long>();

        foreach (var id in ids)
        {
            if (result.Any(v => v.OrderId == id) == false)
            {
                notFound.Add(id);
            }
        }

        if (notFound.Count > 0)
        {
            throw new NotFoundException($"Адреса с ID = ({string.Join(", ", notFound)}) не найдены");
        }

        return result;
    }

    public async Task<Address[]> GetAll(CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table};";

        await using var connection = GetConnectionByShardKey(0);
        
        var cmd = new CommandDefinition(
            sql,
            cancellationToken: token);

        var dto = await connection.QueryAsync<PostgresAddress>(cmd);
        
        return dto.Select(a => a.ToDomain()).ToArray();
    }

    public async Task<Address> Add(Address address, CancellationToken token)
    {
        const string sql = @$"
            insert into {Table}({FieldsForInsert})
            values (:region_id, :order_id, :customer_id, :city, :street, :building, :apartment, :latitude, :longitude)
            returning id;";
        
        await using var connection = GetConnectionByShardKey(address.CustomerId);
        
        var param = new DynamicParameters();
        param.Add("region_id", address.RegionId);
        param.Add("order_id", address.OrderId);
        param.Add("customer_id", address.CustomerId);
        param.Add("city", address.City);
        param.Add("street", address.Street);
        param.Add("building", address.Building);
        param.Add("apartment", address.Apartment);
        param.Add("latitude", address.Latitude);
        param.Add("longitude", address.Longitude);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        await connection.ExecuteAsync(cmd);

        return address;
    }
}