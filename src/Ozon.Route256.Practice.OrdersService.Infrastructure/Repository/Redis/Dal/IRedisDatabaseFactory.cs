using StackExchange.Redis;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Redis.Dal;

public interface IRedisDatabaseFactory
{
    IDatabase GetDatabase();
    IServer GetServer();
}