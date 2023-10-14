using Ozon.Route256.Practice.OrdersService.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Kafka.Producer;

namespace Ozon.Route256.Practice.OrdersService.Kafka;

public static class KafkaExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services
            .AddOptions<KafkaProducerConfig>()
            .Configure<IConfiguration>((opt, config) => config
                .GetSection("Kafka:Producers:NewOrder")
                .Bind(opt));

        services.AddSingleton<IProducerProvider, ProducerProvider>();
        services.AddScoped<NewOrderProducer>();
        
        services
            .AddOptions<PreOrderConsumerConfig>()
            .Configure<IConfiguration>((opt, config) => config
                .GetSection("Kafka:Consumers:PreOrder")
                .Bind(opt));

        services.AddSingleton<IConsumerProvider, ConsumerProvider>();
        services.AddHostedService<PreOrderConsumer>();

        return services;
    }
}