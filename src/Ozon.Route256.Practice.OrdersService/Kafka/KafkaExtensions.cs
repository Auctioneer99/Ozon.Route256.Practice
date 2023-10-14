using Ozon.Route256.Practice.OrdersService.Kafka.Consumer;

namespace Ozon.Route256.Practice.OrdersService.Kafka;

public static class KafkaExtensions
{
    public static IServiceCollection AddConsumers(this IServiceCollection services)
    {
        services
            .AddOptions<PreOrderConsumerConfig>()
            .Configure<IConfiguration>((opt, config) => config
                .GetSection("Kafka:Consumers:PreOrder")
                .Bind(opt));

        services
            .AddSingleton<IConsumerProvider, ConsumerProvider>();

        services.AddHostedService<PreOrderConsumer>();

        return services;
    }
}