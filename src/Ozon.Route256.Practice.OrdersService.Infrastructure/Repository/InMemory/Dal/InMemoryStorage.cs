using System.Collections.Concurrent;
using Ozon.Route256.Practice.OrdersService.Domain.Models;

namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.InMemory.Dal;

public sealed class InMemoryStorage
{
    public readonly ConcurrentDictionary<long, Order> Orders = new(2, 10);
    public readonly ConcurrentDictionary<long, Address> Addresses = new(2, 10);
    public readonly ConcurrentDictionary<long, Region> Regions = new(2, 10);

    public long AddressIdSequence { get; set; }

    public InMemoryStorage()
    {
        FakeOrders();
        FakeAddresses();
        FakeRegions();
    }

    private void FakeOrders()
    {
        var orders = Enumerable.Range(1, 100)
            .Select(x => new Order(
                x,
                Faker.RandomNumber.Next(1, 10),
                Faker.RandomNumber.Next(100, 2000),
                Faker.RandomNumber.Next(1, 20),
                Faker.Enum.Random<OrderType>(),
                Faker.Enum.Random<OrderState>(),
                Faker.RandomNumber.Next(1, 3),
                Faker.RandomNumber.Next(1, 100),
                Faker.Identification.DateOfBirth()
            ));

        foreach (var o in orders)
            Orders[o.Id] = o;
    }

    private void FakeAddresses()
    {
        var addresses = Enumerable.Range(1, 50)
            .Select(x => new Address(
                x,
                Faker.RandomNumber.Next(1, 3),
                Faker.RandomNumber.Next(1, 50),
                Faker.RandomNumber.Next(1, 100),
                Faker.Address.City(),
                Faker.Address.StreetName(),
                Faker.RandomNumber.Next().ToString(),
                Faker.RandomNumber.Next().ToString(),
                (Faker.RandomNumber.Next() / (decimal)int.MaxValue) * 180,
                (Faker.RandomNumber.Next() / (decimal)int.MaxValue) * 90
            ));

        foreach (var a in addresses)
            Addresses[a.Id] = a;

        AddressIdSequence = 51;
    }

    private void FakeRegions()
    {
        Regions[1] = new Region(1, "Moscow", (decimal)55.7522, (decimal)37.6156);
        Regions[2] = new Region(2, "StPetersburg", (decimal)55.01, (decimal)82.55);
        Regions[3] = new Region(3, "Novosibirsk", (decimal)45.32, (decimal)68.23);
    }
}