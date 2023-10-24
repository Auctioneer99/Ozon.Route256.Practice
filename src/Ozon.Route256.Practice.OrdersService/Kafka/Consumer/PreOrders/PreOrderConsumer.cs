using System.Text.Json;
using System.Transactions;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer.PreOrders.Models;
using Ozon.Route256.Practice.OrdersService.Kafka.Producer;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;
using Ozon.Route256.Practice.OrdersService.Services.Mapping;
using IsolationLevel = Confluent.Kafka.IsolationLevel;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.PreOrders;

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

    public async Task<bool> HandleValue(ConsumeResult<string, string> consumeResult, CancellationToken token)
    {
        PreOrder? preOrder;

        try
        {
            preOrder = JsonSerializer.Deserialize<PreOrder>(consumeResult.Message.Value);
        }
        catch (Exception ex)
        {
            return false;
        }

        if (preOrder == null)
        {
            return false;
        }

        using var scope = _scopeFactory.CreateScope();

        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var regionRepository = scope.ServiceProvider.GetRequiredService<IRegionRepository>();
        var addressRepository = scope.ServiceProvider.GetRequiredService<IAddressRepository>();

        var regionFrom = await regionRepository.FindByName(preOrder.Customer.Address.Region, token);
        if (regionFrom == null)
        {
            return false;
        }
        var address = await addressRepository.FindByCoordinates(
            preOrder.Customer.Address.Latitude, 
            preOrder.Customer.Address.Longitude, 
            token);
        
        using (var ts = new TransactionScope(
                   TransactionScopeOption.Required,
                   new TransactionOptions
                   {
                       IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted,
                       Timeout = TimeSpan.FromSeconds(5)
                   },
                   TransactionScopeAsyncFlowOption.Enabled
               ))
        {
            if (address == null)
            {
                var addressDto = preOrder.Customer.Address.ToDto(regionFrom.Id, 0);
                address = await addressRepository.Add(addressDto, token);
            }
        
            var order = preOrder.ToDto(regionFrom.Id, consumeResult.Message.Timestamp.UtcDateTime);
            await orderRepository.Add(order, token);
            
            ts.Complete();
        }

        var validator = scope.ServiceProvider.GetRequiredService<NewOrderValidator>();
        if (validator.ValidateDistance(preOrder.Customer.Address, regionFrom) == false)
        {
            _logger.LogInformation("Пропускаем заказ, растояние больше 5000");
            return false;
        }
        
        var producer = scope.ServiceProvider.GetRequiredService<NewOrderProducer>();
        await producer.Produce(preOrder.Id, token);

        return true;
    }
}