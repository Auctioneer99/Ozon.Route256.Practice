using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer;

public sealed class ConsumerProvider : IConsumerProvider
{
    public IConsumer<string, string> Create(ConsumerConfig config)
    {
        return new ConsumerBuilder<string, string>(config)
            .Build();
    }
}