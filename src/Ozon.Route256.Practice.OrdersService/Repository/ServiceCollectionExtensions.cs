using Ozon.Route256.Practice.OrdersService.Repository.Impl;

namespace Ozon.Route256.Practice.OrdersService.Repository;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStorage>();
        
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ILogisticsRepository, LogisticsRepository>();

        return services;
    }
}