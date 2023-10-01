using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public sealed class OrdersAggregationRequest
{
    [JsonProperty("fromDate")]
    public DateTime FromDate { get; init; }
    
    [JsonProperty("regions")]
    public string[] Regions { get; init; }
}