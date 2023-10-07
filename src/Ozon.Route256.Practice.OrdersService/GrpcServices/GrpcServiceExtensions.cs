﻿using Grpc.Core;
using Ozon.Route256.Practice.LogisticsSimulator.Grpc;
using Ozon.Route256.Practice.OrdersService.ClientBalancing;
using Ozon.Route256.Practice.OrdersService.Infrastructure;
using Ozon.Route256.Practice.OrdersService.Services.Validation;
using Ozon.Route256.Practice.ServiceDiscovery;

namespace Ozon.Route256.Practice.OrdersService.Controllers;

public static class GrpcServiceExtensions
{
    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(options =>
                options.Address = new Uri(configuration.GetValue<string>("ROUTE256_LOGISTICS_SIMULATOR_SERVICE_ADDRESS")))
            .ConfigureChannel(x =>
            {
                x.Credentials = ChannelCredentials.Insecure;
            });
        
        services.AddGrpcClient<Customers.Customers.CustomersClient>(options =>
                options.Address = new Uri(configuration.GetValue<string>("ROUTE256_CUSTOMER_SERVICE_ADDRESS")))
            .ConfigureChannel(x =>
            {
                x.Credentials = ChannelCredentials.Insecure;
            });
        
        services.AddSingleton<IDbStore, DbStore>();
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

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
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