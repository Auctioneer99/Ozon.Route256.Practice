using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class PagingRequest
{
    [JsonProperty("skipCount")]
    public long SkipCount { get; init; }
    
    [JsonProperty("takeCount")]
    public long TakeCount { get; init; }
}