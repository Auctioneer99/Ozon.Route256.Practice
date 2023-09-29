using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class ClientOrdersRequest
{
    [JsonProperty("clientId")]
    public long ClientId { get; set; }
    
    [JsonProperty("from")]
    public DateTime From { get; set; }
    
    [JsonProperty("page")]
    public PagingRequest Page { get; set; }
}