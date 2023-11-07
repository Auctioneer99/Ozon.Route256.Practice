using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;

public interface IShardConnectionFactory
{
    public ShardNpgsqlConnection GetConnectionByBucket(int bucket);

    IEnumerable<int> GetAllBuckets();
}