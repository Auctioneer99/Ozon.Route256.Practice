namespace Ozon.Route256.Practice.OrdersService.Infrastructure.Repository.Redis.Models;

internal sealed record RedisCustomer(
    long Id, 
    string FirstName, 
    string LastName, 
    string MobileNumber, 
    string Email);