using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class Order
{
    [JsonProperty("id")]
    public long Id { get; init; }
    
    [JsonProperty("count")]
    public int Count { get; init; }
    
    [JsonProperty("totalSum")]
    public double TotalSum { get; init; }
    
    [JsonProperty("totalWeight")]
    public double TotalWeight { get; init; }
    
    [JsonProperty("type")]
    public OrderType Type { get; init; }
    
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; init; }
    
    [JsonProperty("regionFrom")]
    public string RegionFrom { get; init; }
    
    [JsonProperty("state")]
    public OrderState State { get; init; }
    
    [JsonProperty("clientName")]
    public string ClientName { get; init; }
    
    [JsonProperty("orderAddress")]
    public Address OrderAddress { get; init; }
    
    [JsonProperty("phone")]
    public string Phone { get; init; }
}