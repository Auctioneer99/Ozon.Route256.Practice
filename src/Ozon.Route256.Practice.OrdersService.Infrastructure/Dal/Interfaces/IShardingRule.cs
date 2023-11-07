namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;

public interface IShardingRule<TKey>
{
    public int GetBucket(TKey value);
}