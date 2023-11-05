using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery;
using Ozon.Route256.Practice.OrdersService.Infrastructure.ServiceDiscovery.Impl;

namespace Ozon.Route256.Practice.OrdersService.Migration.Extensions;

internal static class ApplicationExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IShardsStore, ShardsStore>();
        services.AddSingleton<IServiceDiscoveryRepository, ServiceDiscoveryRepository>();
        
        return services;
    }
}