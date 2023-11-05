namespace Ozon.Route256.Practice.OrdersService.Application.Models;

public sealed record CancelOrderResponse(
    bool Success,
    string Error);