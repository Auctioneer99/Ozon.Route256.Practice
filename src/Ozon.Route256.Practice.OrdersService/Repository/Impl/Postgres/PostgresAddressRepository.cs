using Dapper;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public class PostgresAddressRepository : BaseShardRepository, IAddressRepository
{
    private const string Fields = "id, region_id, order_id, city, street, building, apartment, latitude, longitude";
    private const string FieldsForInsert = "id, region_id, order_id, city, street, building, apartment, latitude, longitude";
    private const string Table = $"{Shards.BucketPlaceholder}.addresses";
     
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
        
        return await connection.QueryFirstAsync<AddressDto?>(cmd);
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
        
        return await connection.QueryFirstAsync<AddressDto?>(cmd);
    }

    public async Task<AddressDto[]> FindManyById(IEnumerable<long> ids, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table};
            where id = any(:ids);";

        await using var connection = GetConnectionByShardKey(0);
        
        var param = new DynamicParameters();
        param.Add("ids", ids);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);
        
        return (await connection.QueryAsync<AddressDto>(cmd)).ToArray();
    }

    public async Task<AddressDto[]> GetManyById(IEnumerable<long> ids, CancellationToken token)
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
            values (:id, :region_id, :order_id, :city, :street, :building, :apartment, :latitude, :longitude)
            returning id;";
        
        await using var connection = GetConnectionByShardKey(dto.OrderId);
        
        var param = new DynamicParameters();
        param.Add("id", dto.Id);
        param.Add("region_id", dto.RegionId);
        param.Add("order_id", dto.OrderId);
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