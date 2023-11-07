using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Application.Services;
using Ozon.Route256.Practice.OrdersService.Application.Services.Impl;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Grpc;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Postgres;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Redis;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Redis.Dal;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Impl;

namespace Ozon.Route256.Practice.OrdersService.Extensions;

internal static class ApplicationExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<IRegionService, RegionService>();

        return services;
    }
    
    public static IServiceCollection AddRepositories(this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<IShardsStore, ShardsStore>();
        services.AddSingleton<IServiceDiscoveryRepository, ServiceDiscoveryRepository>();
        
        services.AddScoped<IRegionRepository, PostgresRegionRepository>();
        services.AddScoped<IAddressRepository, PostgresAddressRepository>();
        services.AddScoped<IOrderRepository, PostgresOrderRepository>();
        
        services.AddSingleton<IRedisDatabaseFactory, RedisDatabaseFactory>(_ => new RedisDatabaseFactory(configuration.GetValue<string>("Redis:ConnectionString")));

        services.AddScoped<ICustomerRepository, RedisCustomerRepository>(provider => new RedisCustomerRepository(
            provider.GetRequiredService<IRedisDatabaseFactory>(),
            provider.GetRequiredService<CustomerRepository>(),
            new TimeSpan(configuration.GetValue<int>("Redis:TTL") * 10_000_000L * 60))
        );

        services.AddScoped<CustomerRepository>();
        services.AddScoped<ILogisticsRepository, LogisticsRepository>();
            
        return services;
    }
}