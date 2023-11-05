using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.HostedServices.Config;

public sealed class OrdersEventConsumerConfig
{
    public string? Topic { get; init; }

    public ConsumerConfig? Config { get; init; }

    public OrdersEventConsumerConfig() { }
    
    public OrdersEventConsumerConfig(string topic, ConsumerConfig config)
    {
        Topic = topic;
        Config = config;
    }
}