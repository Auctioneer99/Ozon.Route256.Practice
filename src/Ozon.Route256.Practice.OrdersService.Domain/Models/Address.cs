namespace Ozon.Route256.Practice.OrdersService.Domain.Models;

public sealed class Address
{
    public long Id { get; }

    public long RegionId { get; private set; }

    public long OrderId { get; }

    public long CustomerId { get; }

    public string City { get; private set; }

    public string Street { get; private set; }

    public string Building { get; private set; }

    public string Apartment { get; private set; }

    public decimal Latitude { get; private set; }
    
    public decimal Longitude { get; private set; }

    public Address(long id, long regionId, long orderId, long customerId, string city, string street, string building,
        string apartment, decimal latitude, decimal longitude)
    {
        Id = id;
        RegionId = regionId;
        OrderId = orderId;
        CustomerId = customerId;
        City = city;
        Street = street;
        Building = building;
        Apartment = apartment;
        Latitude = latitude;
        Longitude = longitude;
    }
}