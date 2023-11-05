using System.Text.Json;
using Confluent.Kafka;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.Route256.Practice.OrdersService.Exceptions;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer.PreOrders;
using Ozon.Route256.Practice.OrdersService.Kafka.Consumer.PreOrders.Models;
using Ozon.Route256.Practice.OrdersService.Kafka.Producer;
using Ozon.Route256.Practice.OrdersService.Repository;
using Ozon.Route256.Practice.OrdersService.Repository.Dto;

namespace Ozon.Route256.Practice.OrdersService.Test;

public class PreOrderKafkaTests
{
    private readonly PreOrderConsumer _consumer;
    
    public PreOrderKafkaTests()
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
    public async Task TestNotFoundRegion()
    {
        await Assert.ThrowsAsync(typeof(NotFoundException), async () =>
        {
            var model = new PreOrder(
                2,
                OrderSource.Web,
                new PreCustomer(2,
                    new PreAddress("23", "asd", "asd", "asd", "asd", 1, 1)),
                Array.Empty<PreGood>());

            var result = await _consumer.HandleValue(CreateMessage("2", JsonSerializer.Serialize(model)),
                CancellationToken.None);
        });
    }
    
    [Fact]
    public async Task TestNotValidDistance()
    {
        var model = new PreOrder(
            2, 
            OrderSource.Web,
            new PreCustomer(2, 
                new PreAddress("1", "asd", "asd", "asd", "asd", 100, 100)), 
            Array.Empty<PreGood>());
        
        var result = await _consumer.HandleValue(CreateMessage("2", JsonSerializer.Serialize(model)), CancellationToken.None);
        
        Assert.False(result);
    }
    
    [Fact]
    public async Task TestValid()
    {
        var model = new PreOrder(
            2, 
            OrderSource.Web,
            new PreCustomer(2, 
                new PreAddress("1", "asd", "asd", "asd", "asd", 1, 1)), 
            Array.Empty<PreGood>());
        
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

    private PreOrderConsumer CreateTestable()
    {
        var logger = A.Fake<ILogger<PreOrderConsumer>>();
        var config = A.Fake<IOptions<PreOrderConsumerConfig>>();

        var provider = A.Fake<IConsumerProvider>();
        var scopeFactory = CreateScopeFactory();
        
        return new PreOrderConsumer(logger, provider, config, scopeFactory);
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
        var regionRepository = CreateRegionRepository();
        var addressRepository = CreateAddressRepository();

        var validator = new NewOrderValidator();
        var orderProducer = A.Dummy<NewOrderProducer>();

        A.CallTo(() => scope.ServiceProvider.GetService(A<Type>._))
            .ReturnsLazily((Type type) =>
            {
                return type.Name switch
                {
                    nameof(IRegionRepository) => regionRepository,
                    nameof(IOrderRepository) => orderRepository,
                    nameof(IAddressRepository) => addressRepository,
                    nameof(NewOrderValidator) => validator,
                    nameof(NewOrderProducer) => orderProducer,
                    _ => throw new NotImplementedException()
                };
            });
        
        return scope;
    }

    private IOrderRepository CreateOrderRepository()
    {
        var orderRepository = A.Fake<IOrderRepository>();

        return orderRepository;
    }

    private IRegionRepository CreateRegionRepository()
    {
        var regionRepository = A.Fake<IRegionRepository>();

        A.CallTo(() => regionRepository.FindByName(A<string>._, A<CancellationToken>._))
            .ReturnsLazily((string region, CancellationToken token) => new RegionDto(region.GetHashCode(), region, 1, 1));
        A.CallTo(() => regionRepository.FindByName(A<string>._, A<CancellationToken>._))
            .WhenArgumentsMatch((string region, CancellationToken token) => long.Parse(region) > 10)
            .ThrowsAsync((string region, CancellationToken token) => new NotFoundException(region.ToString()));

        return regionRepository;
    }

    private IAddressRepository CreateAddressRepository()
    {
        var addressRepository = A.Fake<IAddressRepository>();
        
        A.CallTo(() => addressRepository.FindByCoordinates(A<double>._, A<double>._, A<CancellationToken>._))
            .ReturnsLazily((double lat, double lon, CancellationToken token) =>
                new AddressDto((int)lat, (int)lon, (int)lat, (int)lon, "asd", "asd", "asd", "asd", (decimal)lat, (decimal)lon));

        A.CallTo(() => addressRepository.Add(A<AddressDto>._, A<CancellationToken>._))
            .ReturnsLazily((AddressDto address, CancellationToken token) => address);
        
        return addressRepository;
    }
}