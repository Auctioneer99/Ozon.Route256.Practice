namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public sealed record CustomerDto(
    int Id,
    string FirstName,
    string LastName,
    string MobileNumber,
    string Email
);