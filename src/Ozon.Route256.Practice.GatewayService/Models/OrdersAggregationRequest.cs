using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class OrdersAggregationRequest
{
    [JsonProperty("fromDate")]
    public DateTime FromDate { get; set; }
    
    [JsonProperty("regions")]
    public string[] Regions { get; set; }
}