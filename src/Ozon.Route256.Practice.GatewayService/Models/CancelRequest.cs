using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class CancelRequest
{
    [JsonProperty("id")]
    public long Id { get; init; }
}