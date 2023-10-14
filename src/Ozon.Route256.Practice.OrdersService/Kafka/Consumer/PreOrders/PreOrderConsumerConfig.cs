﻿using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.PreOrders;

public sealed class PreOrderConsumerConfig
{
    public string Topic { get; init; }

    public ConsumerConfig Config { get; init; }
}