﻿using Confluent.Kafka;

namespace Ozon.Route256.Practice.OrdersService.HostedServices.Config;

public class OrdersEventConsumerConfig
{
    public string Topic { get; init; }

    public ConsumerConfig Config { get; init; }
}