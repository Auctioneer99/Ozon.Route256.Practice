using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class RegionsResponse
{
    [JsonProperty("regions")]
    public string[] Regions { get; init; }
}