using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class Order
{
    [JsonProperty("id")]
    public long Id { get; set; }
    
    [JsonProperty("count")]
    public int Count { get; set; }
    
    [JsonProperty("totalSum")]
    public double TotalSum { get; set; }
    
    [JsonProperty("totalWeight")]
    public double TotalWeight { get; set; }
    
    [JsonProperty("type")]
    public OrderType Type { get; set; }
    
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("regionFrom")]
    public string RegionFrom { get; set; }
    
    [JsonProperty("state")]
    public OrderState State { get; set; }
    
    [JsonProperty("clientName")]
    public string ClientName { get; set; }
    
    [JsonProperty("orderAddress")]
    public Address OrderAddress { get; set; }
    
    [JsonProperty("phone")]
    public string Phone { get; set; }
}