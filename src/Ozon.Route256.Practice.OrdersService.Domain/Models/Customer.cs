using Ozon.Route256.Practice.OrdersService.Domain.Models.Primitives;

namespace Ozon.Route256.Practice.OrdersService.Domain.Models;

public sealed class Customer
{
    public long Id { get; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public Phone MobileNumber { get; private set; }

    public Email Email { get; private set; }

    public Customer(long id, string firstName, string lastName, string phone, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        MobileNumber = phone;
        Email = email;
    }
}