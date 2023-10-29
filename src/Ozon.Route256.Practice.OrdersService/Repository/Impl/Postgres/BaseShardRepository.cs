using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public abstract class BaseShardRepository
{
    protected readonly IShardConnectionFactory _connectionFactory;
    protected readonly IShardingRule<long> _longShardingRule;
    protected readonly IShardingRule<string> _stringShardingRule;

    protected BaseShardRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule)
    {
        _connectionFactory = connectionFactory;
        _longShardingRule = longShardingRule;
        _stringShardingRule = stringShardingRule;
    }

    protected ShardNpgsqlConnection GetConnectionByShardKey(
        long shardKey)
    {
        var bucketId = _longShardingRule.GetBucket(shardKey);
        var connection = GetConnectionByBucket(bucketId);
        return connection;
    }

    protected ShardNpgsqlConnection GetConnectionBySearchKey(
        string searchKey)
    {
        var bucketId = _stringShardingRule.GetBucket(searchKey);
        return GetConnectionByBucket(bucketId);
    }

    protected ShardNpgsqlConnection GetConnectionByBucket(
        int bucketId)
    {
        var connection = _connectionFactory.GetConnectionByBucket(bucketId);
        return connection;
    }
}