using StackExchange.Redis;

namespace Ozon.Route256.Practice.OrdersService.Repository.Impl.Redis;

public interface IRedisDatabaseFactory
{
    IDatabase GetDatabase();
    IServer GetServer();
}