using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.OrdersEvents;

public class OrdersEventConsumerConfig
{
    public string Topic { get; init; }

    public ConsumerConfig Config { get; init; }
}