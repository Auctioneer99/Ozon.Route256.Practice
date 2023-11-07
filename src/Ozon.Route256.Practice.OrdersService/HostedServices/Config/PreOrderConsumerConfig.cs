using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.HostedServices.Config;

public sealed class PreOrderConsumerConfig
{
    public string? Topic { get; init; }

    public ConsumerConfig? Config { get; init; }
    
    public PreOrderConsumerConfig() {}
    
    public PreOrderConsumerConfig(string topic, ConsumerConfig config)
    {
        Topic = topic;
        Config = config;
    }
}