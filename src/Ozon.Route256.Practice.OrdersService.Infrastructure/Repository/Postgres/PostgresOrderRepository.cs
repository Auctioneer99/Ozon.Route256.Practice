using Dapper;
using Ozon.Route256.Practice.OrdersService.Application.Models;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Application.Repository.Models;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Domain.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres;

public class PostgresOrderRepository : BaseShardRepository, IOrderRepository
{
    private const string Fields = $"id as {nameof(PostgresOrder.Id)}, count as {nameof(PostgresOrder.Count)}, total_sum as {nameof(PostgresOrder.TotalSum)}, total_weight as {nameof(PostgresOrder.TotalWeight)}, type as {nameof(PostgresOrder.Type)}, state as {nameof(PostgresOrder.State)}, region_from_id as {nameof(PostgresOrder.RegionFromId)}, customer_id as {nameof(PostgresOrder.CustomerId)}, created_at as {nameof(PostgresOrder.CreatedAt)}";
    private const string FieldsForInsert = "id, count, total_sum, total_weight, type, state, region_from_id, customer_id, created_at";
    private const string Table = $"{Shards.BucketPlaceholder}.orders";

    private const string IndexFields = $"order_id as {nameof(OrderIndex.OrderId)}, shard as {nameof(OrderIndex.Shard)}";
    private const string IndexFieldsForInsert = "order_id, shard";
    private const string IndexTable = $"{Shards.BucketPlaceholder}.orders_id_global_index";
    
    public PostgresOrderRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule): base(connectionFactory, longShardingRule, stringShardingRule)
    {
    }
    
    public async Task<Order?> FindById(long orderId, CancellationToken token)
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

            var index = await connection.QueryFirstOrDefaultAsync<OrderIndex?>(cmd);
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
            param.Add("id", orderId);
        
            var cmd = new CommandDefinition(
                sql,
                param,
                cancellationToken: token);
        
            var dto = await connection.QueryFirstAsync<PostgresOrder>(cmd);

            return dto.ToDomain();
        }
    }
    
    public async Task<Order> GetById(long orderId, CancellationToken token)
    {
        var result = await FindById(orderId, token);

        if (result == null)
        {
            throw new NotFoundException($"Заказ с ID = {orderId} не найден");
        }

        return result;
    }

    public async Task<Order[]> GetByCustomerId(OrderByCustomerRepositoryRequest orderRequest, CancellationToken token)
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

        var result = await connection.QueryAsync<PostgresOrder>(cmd);
        
        return result.Select(o => o.ToDomain()).ToArray();
    }

    public async Task<Order[]> GetAll(CancellationToken token)
    {
        var result = new List<Order>();
        
        foreach (var bucket in _connectionFactory.GetAllBuckets())
        {
            const string sql = @$"
                select {Fields}
                from {Table};";
            
            await using var connection = GetConnectionByBucket(bucket);
        
            var cmd = new CommandDefinition(
                sql,
                cancellationToken: token);

            var dto = await connection.QueryAsync<PostgresOrder>(cmd);
            
            result.AddRange(dto.Select(o => o.ToDomain()));
        }

        return result.ToArray();
    }

    public async Task<Order[]> GetAll(OrderRepositoryRequest orderRequest, CancellationToken token)
    {
        var result = new List<Order>();

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

            var dto = await connection.QueryAsync<PostgresOrder>(cmd);
            
            result.AddRange(dto.Select(o => o.ToDomain()));
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
    }

    public async Task Add(Order domain, CancellationToken token)
    {
        var orderShard = 0;

        var order = domain.ToPostgres();
        
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

            var index = await connection.QueryFirstOrDefaultAsync<OrderIndex?>(cmd);
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