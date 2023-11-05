using System.Text;
using Murmur;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Repository;

namespace Ozon.Route256.Practice.OrdersService.Dal.Common.Shard.Rules;

public sealed class StringShardingRule : IShardingRule<string>
{
    private readonly int _bucketsCount;

    public StringShardingRule(
        IShardsStore dbStore)
    {
        _bucketsCount = dbStore.BucketsCount;
    }

    public int GetBucket(
        string shardKey)
    {
        var hash = GetHashCodeFromShardKey(shardKey);
        return Math.Abs(hash) % _bucketsCount;
    }

    private int GetHashCodeFromShardKey(
        string shardKey)
    {
        var bytes = Encoding.UTF8.GetBytes(shardKey);
        var murmur = MurmurHash.Create32();
        var hash = murmur.ComputeHash(bytes);
        return BitConverter.ToInt32(hash);
    }
}