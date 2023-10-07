using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public sealed class CustomerOrdersRequest
{
    [JsonProperty("clientId")]
    public long CustomerId { get; init; }
    
    [JsonProperty("from")]
    public DateTime From { get; init; }
    
    [JsonProperty("page")]
    public PagingRequest Page { get; init; }
}