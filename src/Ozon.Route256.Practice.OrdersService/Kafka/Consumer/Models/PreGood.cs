﻿namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.Models;

public sealed record PreGood(
    long Id,
    string Name,
    int Quantity,
    double Price,
    double Weight);