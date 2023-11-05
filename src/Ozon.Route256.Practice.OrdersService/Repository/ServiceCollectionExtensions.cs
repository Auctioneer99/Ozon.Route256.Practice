using Ozon.Route256.Practice.OrdersService.Repository.Impl.Grpc;
using Ozon.Route256.Practice.OrdersService.Repository.Impl.InMemory;
using Ozon.Route256.Practice.OrdersService.Repository.Impl.Postgres;
using Ozon.Route256.Practice.OrdersService.Repository.Impl.Redis;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services,
        IConfiguration configuration
    )
    {
        //services.AddSingleton<InMemoryStorage>();
        
        //services.AddScoped<IRegionRepository, RegionRepository>();
        //services.AddScoped<IAddressRepository, AddressRepository>();
        //services.AddScoped<IOrderRepository, OrderRepository>();

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