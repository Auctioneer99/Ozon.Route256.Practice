using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class Address
{
    [JsonProperty("region")]
    public string Region { get; init; }
    
    [JsonProperty("city")]
    public string City { get; init; }
    
    [JsonProperty("street")]
    public string Street { get; init; }
    
    [JsonProperty("building")]
    public string Building { get; init; }
    
    [JsonProperty("apartment")]
    public string Apartment { get; init; }
    
    [JsonProperty("latitude")]
    public double Latitude { get; init; }
    
    [JsonProperty("longitude")]
    public double Longitude { get; init; }
}