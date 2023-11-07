using Grpc.Core;
using Ozon.Route256.Practice.OrdersService.Grpc.Customers;
using Ozon.Route256.Practice.OrdersService.Grpc.LogisticsSimulator;
using Ozon.Route256.Practice.OrdersService.Grpc.ServiceDiscovery;
using Ozon.Route256.Practice.OrdersService.GrpcServices;
using Ozon.Route256.Practice.OrdersService.HostedServices;
using Ozon.Route256.Practice.OrdersService.Infrastructure;

namespace Ozon.Route256.Practice.OrdersService.Extensions;

internal static class GrpcServiceExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(options =>
                options.Address = new Uri(configuration.GetValue<string>("ROUTE256_LOGISTICS_SIMULATOR_SERVICE_ADDRESS")))
            .ConfigureChannel(x =>
            {
                x.Credentials = ChannelCredentials.Insecure;
            });
        
        services.AddGrpcClient<Customers.CustomersClient>(options =>
                options.Address = new Uri(configuration.GetValue<string>("ROUTE256_CUSTOMER_SERVICE_ADDRESS")))
            .ConfigureChannel(x =>
            {
                x.Credentials = ChannelCredentials.Insecure;
            });
        
        services.AddHostedService<SdConsumerHostedService>();
        services.AddGrpcClient<SdService.SdServiceClient>(options =>
        {
            var url = configuration.GetValue<string>("ROUTE256_SD_ADDRESS");
            if (string.IsNullOrEmpty(url))
                throw new Exception("ROUTE256_SD_ADDRESS variable is empty");

            options.Address = new Uri(url);
        });
        
        return services;
    }

    public static IServiceCollection AddGrpcServer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(x =>
        {
            x.Interceptors.Add<LoggerInterceptor>();
            x.Interceptors.Add<ValidatorInterceptor>();
        });
        services.AddGrpcReflection();
        services.AddTransient<OrdersGrpcService>();
        services.AddOrdersValidator();
        
        return services;
    }
}