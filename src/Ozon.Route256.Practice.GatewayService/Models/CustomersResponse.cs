using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class CustomersResponse
{
    [JsonProperty("customers")]
    public Customer[] Customers { get; set; }
}