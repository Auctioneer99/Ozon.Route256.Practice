using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard.Rules;

namespace Ozon.Route256.Practice.OrdersService.Extensions;

internal static class DalExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbOptions>(configuration.GetSection(nameof(DbOptions)));
        services.AddSingleton<IShardConnectionFactory, ShardConnectionFactory>();
        
        services.AddSingleton<IShardingRule<long>, LongShardingRule>();
        services.AddSingleton<IShardingRule<string>, StringShardingRule>();
        
        return services;
    }
}