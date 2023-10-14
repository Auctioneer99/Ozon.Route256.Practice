using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer.OrdersEvents.Models;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Services.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.OrdersEvents;

public sealed class OrdersEventConsumer : BackgroundService
{
    private readonly ILogger<OrdersEventConsumer> _logger;
    private readonly IConsumerProvider _consumerProvider;
    private readonly IOptions<OrdersEventConsumerConfig> _config;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrdersEventConsumer(
        ILogger<OrdersEventConsumer> logger, 
        IConsumerProvider consumerProvider, 
        IOptions<OrdersEventConsumerConfig> config, 
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _consumerProvider = consumerProvider;
        _config = config;
        _scopeFactory = scopeFactory;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() => HandleExecute(stoppingToken), stoppingToken);
    }

    private async Task HandleExecute(CancellationToken token)
    {
        while (token.IsCancellationRequested == false)
        {
            using var consumer = _consumerProvider.Create(_config.Value.Config);

            try
            {
                consumer.Subscribe(_config.Value.Topic);

                await ConsumeCycle(consumer, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consumer error");

                try
                {
                    consumer.Unsubscribe();
                }
                catch
                {
                    // ignored
                }

                await Task.Delay(TimeSpan.FromSeconds(1), token);
            }
        }
    }

    private async Task ConsumeCycle(IConsumer<string, string> consumer, CancellationToken token)
    {
        while (token.IsCancellationRequested == false)
        {
            var consumeResult = consumer.Consume(token);
            await HandleValue(consumeResult, token);
            consumer.Commit();
        }
    }

    private async Task HandleValue(ConsumeResult<string, string> consumeResult, CancellationToken token)
    {
        var ordersEvent = JsonSerializer.Deserialize<OrdersEvent>(consumeResult.Message.Value);

        if (ordersEvent == null)
        {
            return;
        }
        
        using var scope = _scopeFactory.CreateScope();
        
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

        try
        {
            await orderRepository.UpdateOrderStatus(ordersEvent.OrderId, ordersEvent.OrderState.ToDto(), token);
        }
        catch (NotFoundException exception)
        {
            // ignored
        }
    }
}