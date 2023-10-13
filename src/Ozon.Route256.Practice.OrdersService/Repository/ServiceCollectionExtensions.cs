using Ozon.Route256.Practice.OrdersService.Repository.Impl;
using Ozon.Route256.Practice.OrdersService.Repository.Impl.Redis;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddSingleton<InMemoryStorage>();
        
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        
        services.AddSingleton<IRedisDatabaseFactory, RedisDatabaseFactory>(_ => new RedisDatabaseFactory(configuration.GetValue<string>("Redis:ConnectionString")));

        services.AddTransient<ICustomerRepository, RedisCustomerRepository>(provider => new RedisCustomerRepository(
            provider.GetRequiredService<IRedisDatabaseFactory>(),
            provider.GetRequiredService<CustomerRepository>(),
            new TimeSpan(configuration.GetValue<int>("Redis:TTL") * 10_000_000 * 60))
        );

        services.AddTransient<CustomerRepository>();
        services.AddScoped<ILogisticsRepository, LogisticsRepository>();
            
        return services;
    }
}