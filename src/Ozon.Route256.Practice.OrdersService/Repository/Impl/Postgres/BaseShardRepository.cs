using System.Data.Common;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;

public abstract class BaseShardRepository
{
    protected readonly IShardConnectionFactory _connectionFactory;
    private readonly IShardingRule<long> _longShardingRule;
    private readonly IShardingRule<string> _stringShardingRule;

    protected BaseShardRepository(
        IShardConnectionFactory connectionFactory,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule)
    {
        _connectionFactory = connectionFactory;
        _longShardingRule = longShardingRule;
        _stringShardingRule = stringShardingRule;
    }

    protected DbConnection GetConnectionByShardKey(
        long shardKey)
    {
        var bucketId = _longShardingRule.GetBucket(shardKey);
        var connection = GetConnectionByBucket(bucketId);
        return connection;
    }

    protected DbConnection GetConnectionBySearchKey(
        string searchKey)
    {
        var bucketId = _stringShardingRule.GetBucket(searchKey);
        return GetConnectionByBucket(bucketId);
    }

    protected DbConnection GetConnectionByBucket(
        int bucketId)
    {
        var connection = _connectionFactory.GetConnectionByBucket(bucketId);
        return connection;
    }
}