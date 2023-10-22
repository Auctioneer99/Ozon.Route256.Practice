using System.Data;
using Npgsql;
using NpgsqlTypes;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public class PostgresAddressRepository : IAddressRepository
{
    private readonly IPostgresConnectionFactory _factory;

    private const string Fields = "id, region_id, city, street, building, apartment, latitude, longitude";
    private const string FieldsForInsert = "region_id, city, street, building, apartment, latitude, longitude";
    private const string Table = "addresses";
    
    public PostgresAddressRepository(IPostgresConnectionFactory factory)
    {
        _factory = factory;
    }
    
    public async Task<AddressDto?> FindById(int id, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id;";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("id", id);
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);

        var result = await Read(reader, token);
        return result.FirstOrDefault();
    }

    public async Task<AddressDto?> FindByCoordinates(double latitude, double longitude, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where latitude > :latitude - 0.01 and latitude < :latitude + 0.01
                and longitude > :longitude - 0.01 and longitude < :longitude + 0.01;";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("latitude", latitude);
        command.Parameters.Add("longitude", longitude);
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);

        var result = await Read(reader, token);
        return result.FirstOrDefault();
    }

    public async Task<AddressDto[]> FindManyById(IEnumerable<long> ids, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table};
            where id = any(:ids);";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("ids", ids.ToList());
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await Read(reader, token);
        return result;
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

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await Read(reader, token);
        return result;
    }

    public async Task<AddressDto> Add(AddressDto dto, CancellationToken token)
    {
        const string sql = @$"
            insert into {Table}({FieldsForInsert})
            select {FieldsForInsert} from unnest(:models)
            returning id;";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.Add("models", new[] { dto });
        
        await connection.OpenAsync(token);
        var id = (long)await command.ExecuteScalarAsync(token);

        return dto with { Id = id };
    }

    private async Task<AddressDto[]> Read(NpgsqlDataReader reader, CancellationToken token)
    {
        var result = new List<AddressDto>();
        
        while (await reader.ReadAsync(token))
        {
            result.Add(
                new AddressDto(
                    Id: reader.GetInt64(0),
                    RegionId: reader.GetInt64(1),
                    City: reader.GetString(2),
                    Street: reader.GetString(3),
                    Building: reader.GetString(4),
                    Apartment: reader.GetString(5),
                    Latitude: reader.GetDouble(6),
                    Longitude: reader.GetDouble(7)));
        }

        return result.ToArray();
    }
}