using Ozon.Route256.Practice.OrdersService.HostedServices;
using Ozon.Route256.Practice.OrdersService.HostedServices.Config;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.OrdersEvents;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;
using Ozon.Route256.Practice.OrdersService.Kafka.Producer;

namespace Ozon.Route256.Practice.OrdersService.Extensions;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafka(this IServiceCollection services)
    {
        services
            .AddOptions<KafkaProducerConfig>()
            .Configure<IConfiguration>((opt, config) => config
                .GetSection("Kafka:Producers:NewOrder")
                .Bind(opt));

        services.AddSingleton<IProducerProvider, ProducerProvider>();
        services.AddScoped<NewOrderProducer>();
        services.AddScoped<NewOrderValidator>();
        
        
        services.AddSingleton<IConsumerProvider, ConsumerProvider>();
        
        services
            .AddOptions<PreOrderConsumerConfig>()
            .Configure<IConfiguration>((opt, config) => config
                .GetSection("Kafka:Consumers:PreOrder")
                .Bind(opt));

        services.AddTransient<PreOrdersService>();
        services.AddHostedService<PreOrderConsumer>();

        services
            .AddOptions<OrdersEventConsumerConfig>()
            .Configure<IConfiguration>((opt, config) => config
                .GetSection("Kafka:Consumers:OrdersEvent")
                .Bind(opt));

        services.AddTransient<OrdersEventsService>();
        services.AddHostedService<OrdersEventConsumer>();
        
        return services;
    }
}