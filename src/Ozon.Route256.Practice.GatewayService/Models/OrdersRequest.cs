using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class OrdersRequest
{
    [JsonProperty("regionFilter")]
    public string[] RegionFilter { get; set; }
    
    [JsonProperty("orderTypeFilter")]
    public OrderType OrderTypeFilter { get; set; }
    
    [JsonProperty("page")]
    public PagingRequest Page { get; set; }

    [JsonProperty("sort")]
    public SortType Sort { get; set; }
    
    [JsonProperty("sortField")]
    public OrderFilterField SortField { get; set; }
}