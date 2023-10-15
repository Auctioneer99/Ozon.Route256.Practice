using System.Data;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public class PostgresRegionRepository : IRegionRepository
{
    private readonly IPostgresConnectionFactory _factory;

    private const string Fields = "id, name, latitude, longitude";
    private const string Table = "regions";
    
    public PostgresRegionRepository(IPostgresConnectionFactory factory)
    {
        _factory = factory;
    }
    
    public async Task<RegionDto?> FindById(long id, CancellationToken token)
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

    public async Task<RegionDto?> FindByName(string name, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where name = :name;";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("name", name);
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, token);

        var result = await Read(reader, token);
        return result.FirstOrDefault();
    }

    public async Task<RegionDto[]> FindManyById(IEnumerable<long> ids, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where id in (:ids);";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("ids", ids.ToList());
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await Read(reader, token);
        return result; 
    }

    public async Task<RegionDto[]> FindManyByName(IEnumerable<string> names, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where name in (:names);";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("names", names.ToList());
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await Read(reader, token);
        return result; 
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

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await Read(reader, token);
        return result; 
    }

    private async Task<RegionDto[]> Read(NpgsqlDataReader reader, CancellationToken token)
    {
        var result = new List<RegionDto>();
        
        while (await reader.ReadAsync(token))
        {
            result.Add(
                new RegionDto(
                    Id: reader.GetInt64(0),
                    Name: reader.GetString(1),
                    Latitude: reader.GetDouble(2),
                    Longitude: reader.GetDouble(3)));
        }

        return result.ToArray();
    }
}