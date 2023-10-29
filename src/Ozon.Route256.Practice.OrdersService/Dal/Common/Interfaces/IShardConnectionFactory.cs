using Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;

public interface IShardConnectionFactory
{
    public ShardNpgsqlConnection GetConnectionByBucket(int bucket);

    IEnumerable<int> GetAllBuckets();
}