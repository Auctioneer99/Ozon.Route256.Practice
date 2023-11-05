namespace Ozon.Route256.Practice.OrdersService.Domain.Models;

public sealed class Customer
{
    public long Id { get; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string MobileNumber { get; private set; }

    public string Email { get; private set; }

    public Customer(long id, string firstName, string lastName, string phone, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        MobileNumber = phone;
        Email = email;
    }
}