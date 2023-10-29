using System.Data;
using System.Linq.Expressions;
using Dapper;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public class PostgresOrderRepository : BaseShardRepository, IOrderRepository
{
    public PostgresOrderRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule): base(connectionFactory, longShardingRule, stringShardingRule)
    {
    }

    private const string Fields = $"id as {nameof(OrderDto.Id)}, count as {nameof(OrderDto.Count)}, total_sum as {nameof(OrderDto.TotalSum)}, total_weight as {nameof(OrderDto.TotalWeight)}, type as {nameof(OrderDto.Type)}, state as {nameof(OrderDto.State)}, region_from_id as {nameof(OrderDto.RegionFromId)}, customer_id as {nameof(OrderDto.CustomerId)}, created_at as {nameof(OrderDto.CreatedAt)}";
    private const string FieldsForInsert = "id, count, total_sum, total_weight, type, state, region_from_id, customer_id, created_at";
    private const string Table = $"{Shards.BucketPlaceholder}.orders";

    private const string IndexFields = $"order_id as {nameof(OrderIndexDto.OrderId)}, shard as {nameof(OrderIndexDto.Shard)}";
    private const string IndexFieldsForInsert = "order_id, shard";
    private const string IndexTable = $"{Shards.BucketPlaceholder}.orders_id_global_index";
    
    public async Task<OrderDto?> FindById(long id, CancellationToken token)
    {
        var bucket = 0;
        await using (var connection = GetConnectionByShardKey(id))
        {
            const string sql = @$"
                select {IndexFields}
                from {IndexTable}
                where order_id = :id;";
            var param = new DynamicParameters();
            param.Add("id", id);
        
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);

            var index = await connection.QueryFirstOrDefaultAsync<OrderIndexDto?>(cmd);
            if (index == null)
            {
                return null;
            }
            bucket = index.Shard;
        }
        
        await using(var connection = GetConnectionByBucket(bucket))
        {
            const string sql = @$"
            select {Fields}
            from {Table}
            where id = :id;";

        
            var param = new DynamicParameters();
            param.Add("id", id);
        
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);
        
            return await connection.QueryFirstOrDefaultAsync<OrderDto?>(cmd);
        }
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
            limit :limit offset :skip;";

        await using var connection = GetConnectionByShardKey(orderRequest.CustomerId);
        
        var param = new DynamicParameters();
        param.Add("id", orderRequest.CustomerId);
        param.Add("from", orderRequest.From);
        param.Add("limit", orderRequest.TakeCount);
        param.Add("skip", orderRequest.SkipCount);
        
        var cmd = new CommandDefinition(
            sql,
            param,
            cancellationToken: token);

        var result = await connection.QueryAsync<OrderDto>(cmd);
        
        return result.ToArray();
    }

    public async Task<OrderDto[]> GetAll(CancellationToken token)
    {
        var result = new List<OrderDto>();
        
        foreach (var bucket in _connectionFactory.GetAllBuckets())
        {
            const string sql = @$"
                select {Fields}
                from {Table};";
            
            await using var connection = GetConnectionByBucket(bucket);
        
            var cmd = new CommandDefinition(
                sql,
                cancellationToken: token);
        
            result.AddRange(await connection.QueryAsync<OrderDto>(cmd));
        }

        return result.ToArray();
    }

    public async Task<OrderDto[]> GetAll(OrderRequestDto orderRequest, CancellationToken token)
    {
        var result = new List<OrderDto>();

        foreach (var bucket in _connectionFactory.GetAllBuckets())
        {
            string sql = @$"
                select {Fields}
                from {Table}
                where {(orderRequest.OrderType != OrderType.Undefined ? "type = :type and " : "")}
                    region_from_id = any(:regions) and created_at > :from";

            await using var connection = GetConnectionByBucket(bucket);

            var param = new DynamicParameters();
            param.Add("type", orderRequest.OrderType);
            param.Add("regions", orderRequest.Regions.ToList());
            param.Add("from", orderRequest.From);
            
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);

            result.AddRange(await connection.QueryAsync<OrderDto>(cmd));
        }

        var ordered = result.AsEnumerable();
        
        switch (orderRequest.OrderField)
        {
            case OrderField.NoneField:
                break;
            case OrderField.Id:
                ordered = Order(ordered, o => o.Id, orderRequest.SortingType);
                break;
            case OrderField.Count:
                ordered = Order(ordered, o => o.Count, orderRequest.SortingType);
                break;
            case OrderField.TotalSum:
                ordered = Order(ordered, o => o.TotalSum, orderRequest.SortingType);
                break;
            case OrderField.TotalWeight:
                ordered = Order(ordered, o => o.TotalWeight, orderRequest.SortingType);
                break;
            case OrderField.OrderType:
                ordered = Order(ordered, o => o.Type, orderRequest.SortingType);
                break;
            case OrderField.CreatedAt:
                ordered = Order(ordered, o => o.CreatedAt, orderRequest.SortingType);
                break;
            case OrderField.OrderState:
                ordered = Order(ordered, o => o.State, orderRequest.SortingType);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        ordered = ordered.Skip((int)orderRequest.SkipCount);

        if (orderRequest.TakeCount > 0)
        {
            ordered = ordered.Take((int)orderRequest.TakeCount);
        }

        return ordered.ToArray();

        IEnumerable<T> Order<T>(IEnumerable<T> enumerable, Func<T, object> field, SortingType order)
        {
            switch (order)
            {
                case SortingType.None:
                    return enumerable.OrderBy(field);
                case SortingType.Ascending:
                    return enumerable.OrderBy(field);
                case SortingType.Descending:
                    return enumerable.OrderByDescending(field);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        throw new NotImplementedException();
        /*string orderField = null;
        
        

        string orderDir = null;
        

        string sql = @$"
            select {Fields}
            from {Table}
            where {(orderRequest.OrderType != OrderType.Undefined ? "type = :type and " : "")}
                region_from_id = any(:regions)
            {(orderField != null ? $"order by {orderField} {orderDir}" : "")}";

        if (orderRequest.TakeCount > 0)
        {
            sql += " limit :limit";
        }
        
        sql += " offset :offset;";

        await using var connection = _factory.GetConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.Add("regions", orderRequest.Regions.ToList());
        command.Parameters.Add("type", orderRequest.OrderType);
        command.Parameters.Add("limit", orderRequest.TakeCount);
        command.Parameters.Add("offset", orderRequest.SkipCount);
        
        await connection.OpenAsync(token);
        await using var reader = await command.ExecuteReaderAsync(token);

        var result = await Read(reader, token);
        return result;*/
    }

    public async Task Add(OrderDto order, CancellationToken token)
    {
        var orderShard = 0;
        
        await using (var connection = GetConnectionByShardKey(order.CustomerId))
        {
            orderShard = connection.Bucket;
            
            const string sql = @$"
            insert into {Table}({FieldsForInsert})
            values (:id, :count, :total_sum, :total_weight, :type, :state, :region_from_id, :customer_id, :created_at);";
            
            var param = new DynamicParameters();
            param.Add("id", order.Id);
            param.Add("count", order.Count);
            param.Add("total_sum", order.TotalSum);
            param.Add("total_weight", order.TotalWeight);
            param.Add("type", order.Type);
            param.Add("state", order.State);
            param.Add("region_from_id", order.RegionFromId);
            param.Add("customer_id", order.CustomerId);
            param.Add("created_at", order.CreatedAt);
        
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);
        
            await connection.ExecuteAsync(cmd);
        }

        await using (var connection = GetConnectionByShardKey(order.Id))
        {
            const string sql = @$"
            insert into {IndexTable}({IndexFieldsForInsert})
            values (:order_id, :shard);";
            
            var param = new DynamicParameters();
            param.Add("order_id", order.Id);
            param.Add("shard", orderShard);
        
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);
        
            await connection.ExecuteAsync(cmd);
        }
    }

    public async Task UpdateOrderStatus(long orderId, OrderState state, CancellationToken token)
    {
        var bucket = 0;
        await using (var connection = GetConnectionByShardKey(orderId))
        {
            const string sql = @$"
                select {IndexFields}
                from {IndexTable}
                where order_id = :id;";
            var param = new DynamicParameters();
            param.Add("id", orderId);
        
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);

            var index = await connection.QueryFirstOrDefaultAsync<OrderIndexDto?>(cmd);
            if (index == null)
            {
                throw new NotFoundException($"Не найден с заказ Id {orderId}");
            }
            bucket = index.Shard;
        }
        
        await using(var connection = GetConnectionByBucket(bucket))
        {
            const string sql = @$"
                update {Table} set state = :state
                where id = :id;";

            var param = new DynamicParameters();
            param.Add("id", orderId);
            param.Add("state", state);
        
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);
        
            await connection.ExecuteAsync(cmd);
        }
    }
}