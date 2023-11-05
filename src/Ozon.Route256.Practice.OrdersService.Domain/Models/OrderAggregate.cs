namespace Ozon.Route256.Practice.OrdersService.Domain.Models;

public sealed class OrderAggregate
{
    public Customer Customer { get; private set; }
    
    public Order Order { get; private set; }
    
    public Address Address { get; private set; }
    
    public Region Region { get; private set; }

    public OrderAggregate(Customer customer, Order order, Address address, Region region)
    {
        Customer = customer;
        Order = order;
        Address = address;
        Region = region;
    }
}