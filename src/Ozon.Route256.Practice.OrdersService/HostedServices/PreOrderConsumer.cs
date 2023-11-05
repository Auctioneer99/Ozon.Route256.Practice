using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.HostedServices.Config;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders.Models;

namespace Ozon.Route256.Practice.OrdersService.HostedServices;

public sealed class PreOrderConsumer : BackgroundService
{
    private readonly ILogger<PreOrderConsumer> _logger;
    private readonly IConsumerProvider _consumerProvider;
    private readonly IOptions<PreOrderConsumerConfig> _config;
    private readonly IServiceScopeFactory _scopeFactory;

    public PreOrderConsumer(
        ILogger<PreOrderConsumer> logger, 
        IConsumerProvider consumerProvider, 
        IOptions<PreOrderConsumerConfig> config, 
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
            using var consumer = _consumerProvider.Create(_config.Value.Config!);

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
        PreOrder? preOrder;

        try
        {
            preOrder = JsonSerializer.Deserialize<PreOrder>(consumeResult.Message.Value);
        }
        catch
        {
            return false;
        }

        if (preOrder == null)
        {
            return false;
        }

        using var scope = _scopeFactory.CreateScope();

        var service  = scope.ServiceProvider.GetRequiredService<PreOrdersService>();

        return await service.Handle(preOrder, consumeResult.Message.Timestamp.UtcDateTime, token);
    }
}