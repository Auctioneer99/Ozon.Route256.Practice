using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Practice.OrdersService.Grpc.ServiceDiscovery;

namespace Ozon.Route256.Practice.OrdersService.Migration.Extensions;

internal static class GrpcServiceExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<SdService.SdServiceClient>(options =>
        {
            var url = configuration.GetValue<string>("ROUTE256_SD_ADDRESS");
            if (string.IsNullOrEmpty(url))
                throw new Exception("ROUTE256_SD_ADDRESS variable is empty");

            options.Address = new Uri(url);
        });
        
        return services;
    }
}