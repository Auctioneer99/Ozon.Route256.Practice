using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Interfaces;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Dal.Shard;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Migration.Common.Shard;

namespace Ozon.Route256.Practice.OrdersService.Migration.Extensions;

internal static class DalExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbOptions>(configuration.GetSection(nameof(DbOptions)));
        services.AddSingleton<IShardConnectionFactory, ShardConnectionFactory>();
        
        return services;
    }
    
    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services.AddSingleton<IShardMigrator, ShardMigrator>();
        
        return services;
    }
}