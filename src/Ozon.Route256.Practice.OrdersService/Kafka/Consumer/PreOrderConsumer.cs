using System.Runtime.CompilerServices;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer.Models;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Services.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer;

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
        var preOrder = JsonSerializer.Deserialize<PreOrder>(consumeResult.Message.Value);

        if (preOrder == null)
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();

        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var regionRepository = scope.ServiceProvider.GetRequiredService<IRegionRepository>();
        var addressRepository = scope.ServiceProvider.GetRequiredService<IAddressRepository>();

        var regionFrom = await regionRepository.FindByName(preOrder.Customer.Address.Region, token);
        if (regionFrom == null)
        {
            return;
        }
        
        var address = await addressRepository.FindByCoordinates(
            preOrder.Customer.Address.Latitude, 
            preOrder.Customer.Address.Longitude, 
            token);

        if (address == null)
        {
            var addressDto = preOrder.Customer.Address.ToDto(regionFrom.Id);
            address = await addressRepository.Add(addressDto, token);
        }
        
        var order = preOrder.ToDto(regionFrom.Id, address.Id, consumeResult.Message.Timestamp.UtcDateTime);

        await orderRepository.Add(order, token);
    }
}