namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public sealed record AddressDto
(
    long Id,
    long RegionId,
    long OrderId,
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude
);