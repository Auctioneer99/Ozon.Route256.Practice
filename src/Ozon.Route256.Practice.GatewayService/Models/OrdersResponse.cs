using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public sealed class OrdersResponse
{
    [JsonProperty("orders")]
    public Order[] Orders { get; init; }
}