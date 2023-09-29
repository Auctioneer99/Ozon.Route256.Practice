using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class OrdersResponse
{
    [JsonProperty("orders")]
    public Order[] Orders { get; set; }
}