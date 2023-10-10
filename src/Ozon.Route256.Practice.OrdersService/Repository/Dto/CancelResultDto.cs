namespace Ozon.Route256.Practice.OrdersService.Repository.Dto;

public sealed record CancelResultDto(
    bool Success,
    string Error
);