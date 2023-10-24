namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;

public interface IShardingRule<TKey>
{
    public int GetBucket(TKey value);
}