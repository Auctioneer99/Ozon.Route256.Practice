using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class OrdersRequest
{
    [JsonProperty("regionFilter")]
    public string[] RegionFilter { get; init; }
    
    [JsonProperty("orderTypeFilter")]
    public OrderType OrderTypeFilter { get; init; }
    
    [JsonProperty("page")]
    public PagingRequest Page { get; init; }

    [JsonProperty("sort")]
    public SortType Sort { get; init; }
    
    [JsonProperty("sortField")]
    public OrderFilterField SortField { get; init; }
}