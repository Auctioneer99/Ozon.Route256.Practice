namespace Ozon.Route256.Practice.OrdersService.Application.Models;

public sealed record OrdersAggregationRequest(
    string[] Regions, 
    DateTime FromDate);