using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer;

public interface IConsumerProvider
{
    IConsumer<string, string> Create(ConsumerConfig config);
}