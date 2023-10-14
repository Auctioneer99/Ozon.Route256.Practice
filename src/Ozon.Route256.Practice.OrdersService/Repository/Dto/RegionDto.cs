namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public sealed record RegionDto(
    long Id,
    string Name,
    double Latitude,
    double Longitude
);