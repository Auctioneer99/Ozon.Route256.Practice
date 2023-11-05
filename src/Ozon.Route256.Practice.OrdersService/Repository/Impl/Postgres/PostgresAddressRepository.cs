using Dapper;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public class PostgresAddressRepository : BaseShardRepository, IAddressRepository
{
    private const string Fields = $"id as {nameof(AddressDto.Id)}, region_id as {nameof(AddressDto.RegionId)}, order_id as {nameof(AddressDto.OrderId)}, customer_id as {nameof(AddressDto.CustomerId)}, city as {nameof(AddressDto.City)}, street as {nameof(AddressDto.Street)}, building as {nameof(AddressDto.Building)}, apartment as {nameof(AddressDto.Apartment)}, latitude as {nameof(AddressDto.Latitude)}, longitude as {nameof(AddressDto.Longitude)}";
    private const string FieldsForInsert = "region_id, order_id, customer_id, city, street, building, apartment, latitude, longitude";
    private const string Table = $"{Shards.BucketPlaceholder}.addresses";
    
    private const string IndexFields = $"order_id as {nameof(OrderIndexDto.OrderId)}, shard as {nameof(OrderIndexDto.Shard)}";
    private const string IndexFieldsForInsert = "order_id, shard";
    private const string IndexTable = $"{Shards.BucketPlaceholder}.orders_id_global_index";
    
    public PostgresAddressRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule): base(connectionFactory, longShardingRule, stringShardingRule)
    {
    }
    
    public async Task<AddressDto?> FindById(int id, CancellationToken token)
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
        
        return await connection.QueryFirstOrDefaultAsync<AddressDto?>(cmd);
    }

    public async Task<AddressDto?> FindByCoordinates(double latitude, double longitude, CancellationToken token)
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
        
        return await connection.QueryFirstOrDefaultAsync<AddressDto?>(cmd);
    }

    public async Task<AddressDto[]> FindManyByOrderId(IEnumerable<long> ids, CancellationToken token)
    {
        var orderShards = ids.GroupBy(id => _longShardingRule.GetBucket(id));
        var orderIndexes = new List<OrderIndexDto>();
        
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
        
                orderIndexes.AddRange(await connection.QueryAsync<OrderIndexDto>(cmd));
            }
        }

        var addressShards = orderIndexes.GroupBy(o => o.Shard);
        var result = new List<AddressDto>();
        
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
        
                result.AddRange(await connection.QueryAsync<AddressDto>(cmd));
            }
        }

        return result.ToArray();
    }

    public async Task<AddressDto[]> GetManyByOrderId(IEnumerable<long> ids, CancellationToken token)
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

    public async Task<AddressDto[]> GetAll(CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table};";

        await using var connection = GetConnectionByShardKey(0);
        
        var cmd = new CommandDefinition(
            sql,
            cancellationToken: token);
        
        return (await connection.QueryAsync<AddressDto>(cmd)).ToArray();
    }

    public async Task<AddressDto> Add(AddressDto dto, CancellationToken token)
    {
        const string sql = @$"
            insert into {Table}({FieldsForInsert})
            values (:region_id, :order_id, :customer_id, :city, :street, :building, :apartment, :latitude, :longitude)
            returning id;";
        
        await using var connection = GetConnectionByShardKey(dto.CustomerId);
        
        var param = new DynamicParameters();
        param.Add("region_id", dto.RegionId);
        param.Add("order_id", dto.OrderId);
        param.Add("customer_id", dto.CustomerId);
        param.Add("city", dto.City);
        param.Add("street", dto.Street);
        param.Add("building", dto.Building);
        param.Add("apartment", dto.Apartment);
        param.Add("latitude", dto.Latitude);
        param.Add("longitude", dto.Longitude);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        var id = await connection.ExecuteScalarAsync<long>(cmd);

        return dto with { Id = id };
    }
}