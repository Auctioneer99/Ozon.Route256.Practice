using System.Text.Json;
using Confluent.Kafka;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Domain.Exceptions;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.HostedServices;
using Ozon.Route256.Practice.OrdersService.HostedServices.Config;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.OrdersEvents.Models;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer;
using OrderState = Ozon.Route256.Practice.OrdersService.Domain.Models.OrderState;

namespace Ozon.Route256.Practice.OrdersService.Test;

public class OrdersEventKafkaTests
{
    private readonly OrdersEventConsumer _consumer;
    
    public OrdersEventKafkaTests()
    {
        _consumer = CreateTestable();
    }
    
    [Fact]
    public async Task TestNotValidEmptyMessage()
    {
        var result = await _consumer.HandleValue(CreateMessage("1", ""), CancellationToken.None);
        
        Assert.False(result);
    }
    
    [Fact]
    public async Task TestNotValidJsonMessage()
    {
        var result = await _consumer.HandleValue(CreateMessage("2", "not}{valid::json"), CancellationToken.None);
        
        Assert.False(result);
    }
    
    [Fact]
    public async Task TestNotFoundOrder()
    {
        var model = new OrdersEvent(
            100,
            Infrastructure.Kafka.Consumer.OrdersEvents.Models.OrderState.Delivered,
            DateTime.Now);

        var result = await _consumer.HandleValue(CreateMessage("2", JsonSerializer.Serialize(model)),
            CancellationToken.None);
        
        Assert.False(result);
    }
    
    [Fact]
    public async Task TestValid()
    {
        var model = new OrdersEvent(
            2,
            Infrastructure.Kafka.Consumer.OrdersEvents.Models.OrderState.Delivered,
            DateTime.Now);
        
        var result = await _consumer.HandleValue(CreateMessage("2", JsonSerializer.Serialize(model)), CancellationToken.None);
        
        Assert.True(result);
    }
    
    private ConsumeResult<string, string> CreateMessage(string key, string value)
    {
        return new ConsumeResult<string, string>()
        {
            Message = new Message<string, string>()
            {
                Key = key,
                Value = value,
                Timestamp = Timestamp.Default
            }
        };
    }

    private OrdersEventConsumer CreateTestable()
    {
        var logger = A.Fake<ILogger<OrdersEventConsumer>>();
        var config = A.Fake<IOptions<OrdersEventConsumerConfig>>();

        var provider = A.Fake<IConsumerProvider>();
        var scopeFactory = CreateScopeFactory();
        
        return new OrdersEventConsumer(logger, provider, config, scopeFactory);
    }

    private IServiceScopeFactory CreateScopeFactory()
    {
        var scopeFactory = A.Fake<IServiceScopeFactory>();
        
        var scope = CreateScope();

        A.CallTo(() => scopeFactory.CreateScope())
            .Returns(scope);

        return scopeFactory;
    }
    
    private IServiceScope CreateScope()
    {
        var scope = A.Fake<IServiceScope>();

        var orderRepository = CreateOrderRepository();

        A.CallTo(() => scope.ServiceProvider.GetService(A<Type>._))
            .ReturnsLazily((Type type) =>
            {
                return type.Name switch
                {
                    nameof(IOrderRepository) => orderRepository,
                    _ => throw new NotImplementedException()
                };
            });
        
        return scope;
    }

    private IOrderRepository CreateOrderRepository()
    {
        var orderRepository = A.Fake<IOrderRepository>();

        A.CallTo(() => orderRepository.UpdateOrderStatus(A<long>._, A<OrderState>._, A<CancellationToken>._))
            .ReturnsLazily((long id, OrderState state, CancellationToken token) => Task.CompletedTask);
        A.CallTo(() => orderRepository.UpdateOrderStatus(A<long>._, A<OrderState>._, A<CancellationToken>._))
            .WhenArgumentsMatch((long id, OrderState state, CancellationToken token) => id > 10)
            .ThrowsAsync((long id, OrderState state, CancellationToken token) => new NotFoundException(id.ToString()));
        
        return orderRepository;
    }
}