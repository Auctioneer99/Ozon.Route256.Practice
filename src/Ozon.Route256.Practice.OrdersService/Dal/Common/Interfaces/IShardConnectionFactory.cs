using System.Data.Common;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;

public interface IShardConnectionFactory
{
    public DbConnection GetConnectionByBucket(int bucket);

    IEnumerable<int> GetAllBuckets();
}