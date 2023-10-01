using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class StatusResponse
{
    [JsonProperty("status")]
    public string Status { get; init; }
}