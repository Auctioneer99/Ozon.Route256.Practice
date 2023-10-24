using Npgsql;
using Ozon.Route256.Practice.OrdersService.Dal.Common;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Impl;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Interfaces;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Shard;
using Ozon.Route256.Practice.OrdersService.Dal.Common.Shard.Rules;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Dal;

public static class DalExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbOptions>(configuration.GetSection(nameof(DbOptions)));
        services.AddSingleton<IShardConnectionFactory, ShardConnectionFactory>();
        
        services.AddSingleton<IShardingRule<long>, LongShardingRule>();
        services.AddSingleton<IShardingRule<string>, StringShardingRule>();
        
        //var mapper = NpgsqlConnection.GlobalTypeMapper;
        //mapper.MapEnum<OrderState>("order_state");
        //mapper.MapEnum<OrderType>("order_type");
        //mapper.MapComposite<OrderDto>("order");
        //mapper.MapComposite<AddressDto>("address");
        //mapper.MapComposite<RegionDto>("region");
        
        return services;
    }
    
    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        services.AddSingleton<IShardMigrator, ShardMigrator>();
        
        return services;
    }
}