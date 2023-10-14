using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Producer;

public sealed class NewOrderProducer
{
    private readonly IProducerProvider _producerProvider;
    private readonly IOptions<KafkaProducerConfig> _config;

    public NewOrderProducer(IProducerProvider producerProvider, IOptions<KafkaProducerConfig> config)
    {
        _producerProvider = producerProvider;
        _config = config;
    }

    public async Task Produce(long id, CancellationToken token)
    {
        var producer = _producerProvider.Get();
        var message = GetMessage(id);
        
        await producer.ProduceAsync(
            _config.Value.Topic,
            message,
            token);
    }

    private Message<string, string> GetMessage(long id)
    {
        var newOrder = new KafkaNewOrder(id);

        var value = JsonSerializer.Serialize(newOrder);

        return new Message<string, string>()
        {
            Key = id.ToString(),
            Value = value
        };
    }

    private sealed record KafkaNewOrder(long OrderId);
}