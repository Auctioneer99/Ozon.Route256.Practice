using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.HostedServices.Config;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.OrdersEvents;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.OrdersEvents.Models;

namespace Ozon.Route256.Practice.OrdersService.HostedServices;

internal sealed class OrdersEventConsumer : BackgroundService
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

    public async Task<bool> HandleValue(ConsumeResult<string, string> consumeResult, CancellationToken token)
    {
        OrdersEvent? ordersEvent;
        
        try
        {
            ordersEvent = JsonSerializer.Deserialize<OrdersEvent>(consumeResult.Message.Value);
        }
        catch (Exception ex)
        {
            return false;
        }

        if (ordersEvent == null)
        {
            return false;
        }
        
        using var scope = _scopeFactory.CreateScope();
        
        var service = scope.ServiceProvider.GetRequiredService<OrdersEventsService>();

        return await service.Handle(ordersEvent, token);
    }
}