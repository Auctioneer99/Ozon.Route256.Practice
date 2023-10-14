using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Producer;

public sealed class ProducerProvider : IProducerProvider, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public ProducerProvider(IOptions<KafkaProducerConfig> config)
    {
        _producer = new ProducerBuilder<string, string>(config.Value.Config)
            .Build();
    }

    public IProducer<string, string> Get() => _producer;

    public void Dispose() => _producer.Dispose();
}