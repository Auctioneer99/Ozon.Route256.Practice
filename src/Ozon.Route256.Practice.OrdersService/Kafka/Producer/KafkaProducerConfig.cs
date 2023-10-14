using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Producer;

public class KafkaProducerConfig
{
    public string Topic { get; init; }
    
    public ProducerConfig Config { get; init; }
}