using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public sealed class ErrorResponse
{
    [JsonProperty("error")]
    public string Error { get; init; }
}