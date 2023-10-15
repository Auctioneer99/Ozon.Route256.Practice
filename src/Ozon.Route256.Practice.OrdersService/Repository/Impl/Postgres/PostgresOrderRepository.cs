using System.Data;
using Npgsql;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public class PostgresOrderRepository : IOrderRepository
{
    private readonly IPostgresConnectionFactory _factory;

    public PostgresOrderRepository(IPostgresConnectionFactory factory)
    {
        _factory = factory;
    }

    private const string Fields = "id, count, total_sum, total_weight, type, state, region_from_id, customer_id, address_id, created_at";
    private const string FieldsForInsert = "count, total_sum, total_weight, type, state, region_from_id, customer_id, address_id, created_at";
    private const string Table = "orders";
    
    public async Task<OrderDto?> FindById(long id, CancellationToken token)
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

    public async Task<OrderDto> GetById(long id, CancellationToken token)
    {
        var result = await FindById(id, token);

        if (result == null)
        {
            throw new NotFoundException($"Заказ с ID = {id} не найден");
        }

        return result;
    }

    public async Task<OrderDto[]> GetByCustomerId(OrderRequestByCustomerDto orderRequest, CancellationToken token)
    {
        const string sql = @$"
            select {Fields}
            from {Table}
            where customer_id = :id and created_at >= :from
            limit :limit 
            offset :offset;";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.Add("id", orderRequest.CustomerId);
        command.Parameters.Add("from", orderRequest.From);
        command.Parameters.Add("limit", orderRequest.TakeCount);
        command.Parameters.Add("offset", orderRequest.SkipCount);
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await Read(reader, token);
        return result;
    }

    public async Task<OrderDto[]> GetAll(CancellationToken token)
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

    public async Task<OrderDto[]> GetAll(OrderRequestDto orderRequest, CancellationToken token)
    {
        string orderField = null;
        
        switch (orderRequest.OrderField)
        {
            case OrderField.NoneField:
                break;
            case OrderField.Id:
                orderField = "id";
                break;
            case OrderField.Count:
                orderField = "count";
                break;
            case OrderField.TotalSum:
                orderField = "total_sum";
                break;
            case OrderField.TotalWeight:
                orderField = "total_weight";
                break;
            case OrderField.OrderType:
                orderField = "order_type";
                break;
            case OrderField.CreatedAt:
                orderField = "created_at";
                break;
            case OrderField.OrderState:
                orderField = "order_state";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        string orderDir = null;
        switch (orderRequest.SortingType)
        {
            case SortingType.None:
                orderDir = "asc";
                break;
            case SortingType.Ascending:
                orderDir = "asc";
                break;
            case SortingType.Descending:
                orderDir = "desc";
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        string sql = @$"
            select {Fields}
            from {Table}
            where order_type = :type
                and region_id in (:regions)
            {(orderField != null ? $"order by {orderField} {orderDir}" : "")}
            limit :limit
            offset :offset;";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.Add("regions", orderRequest.Regions.ToList());
        command.Parameters.Add("type", orderRequest.OrderType);
        command.Parameters.Add("limit", orderRequest.TakeCount);
        command.Parameters.Add("offset", orderRequest.SkipCount);
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await Read(reader, token);
        return result;
    }

    public async Task Add(OrderDto order, CancellationToken token)
    {
        const string sql = @$"
            insert into {Table}({FieldsForInsert})
            select {FieldsForInsert} from unnest(:models);";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.Add("models", new[] { order });
        
        await connection.OpenAsync(token);
        await command.ExecuteNonQueryAsync(token);
    }

    public async Task UpdateOrderStatus(long orderId, OrderState state, CancellationToken token)
    {
        const string sql = @$"
            update {Table} set order_state = :state
            where id = :id;";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.Add("id", orderId);
        command.Parameters.Add("state", state);
        
        await connection.OpenAsync(token);
        await command.ExecuteNonQueryAsync(token);
    }
    
    private async Task<OrderDto[]> Read(NpgsqlDataReader reader, CancellationToken token)
    {
        var result = new List<OrderDto>();
        
        while (await reader.ReadAsync(token))
        {
            result.Add(
                new OrderDto(
                    Id: reader.GetInt64(0),
                    Count: reader.GetInt32(1),
                    TotalSum: reader.GetDouble(2),
                    TotalWeight: reader.GetDouble(3),
                    Type: reader.GetFieldValue<OrderType>(4),
                    State: reader.GetFieldValue<OrderState>(5),
                    RegionFromId: reader.GetInt64(6),
                    CustomerId: reader.GetInt64(7),
                    AddressId: reader.GetInt64(8),
                    CreatedAt: reader.GetDateTime(9)));
        }

        return result.ToArray();
    }
}