using Microsoft.Extensions.Logging;
using Ozon.Route256.Practice.OrdersService.Application.Repository;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders.Models;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Producer;
using Ozon.Route256.Practice.OrdersService.Infrastructure.Mapping;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Kafka.Consumer.PreOrders;

public sealed class PreOrdersService
{
    private readonly ILogger<PreOrdersService> _logger;
    
    private readonly IOrderRepository _orderRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly IRegionRepository _regionRepository;

    private readonly NewOrderValidator _validator;
    
    private readonly NewOrderProducer _producer;

    public PreOrdersService(
        ILogger<PreOrdersService> logger, 
        IOrderRepository orderRepository, 
        IAddressRepository addressRepository, 
        IRegionRepository regionRepository, 
        NewOrderValidator validator, NewOrderProducer producer)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _addressRepository = addressRepository;
        _regionRepository = regionRepository;
        _validator = validator;
        _producer = producer;
    }

    public async Task<bool> Handle(PreOrder preOrder, DateTime createdAt, CancellationToken token)
    {
        var regionFrom = await _regionRepository.FindByName(preOrder.Customer.Address.Region, token);
        if (regionFrom == null)
        {
            return false;
        }
        
        var order = preOrder.ToDomain(regionFrom.Id, createdAt);
        await _orderRepository.Add(order, token);
        
        var addressDto = preOrder.Customer.Address.ToDomain(regionFrom.Id, order.Id, order.CustomerId);
        await _addressRepository.Add(addressDto, token);

        if (_validator.ValidateDistance(preOrder.Customer.Address, regionFrom) == false)
        {
            _logger.LogInformation("Пропускаем заказ, растояние больше 5000");
            return false;
        }
        
        await _producer.Produce(preOrder.Id, token);

        return true;
    }
}