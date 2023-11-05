using Murmur;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Repository;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Shard.Rules;

public sealed class LongShardingRule : IShardingRule<long>
{
    private readonly int _bucketsCount;

    private readonly Murmur32 _murmur;

    public LongShardingRule(IShardsStore store)
    {
        _murmur = MurmurHash.Create32();
        _bucketsCount = store.BucketsCount;
    }
    
    public int GetBucket(long value)
    {
        var key = GetHashCodeFromKey(value);
        return Math.Abs(key) % _bucketsCount;
    }

    private int GetHashCodeFromKey(long key)
    {
        var bytes = BitConverter.GetBytes(key);
        var hash = _murmur.ComputeHash(bytes);
        return BitConverter.ToInt32(hash);
    }
}