using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class Address
{
    [JsonProperty("region")]
    public string Region { get; set; }
    
    [JsonProperty("city")]
    public string City { get; set; }
    
    [JsonProperty("street")]
    public string Street { get; set; }
    
    [JsonProperty("building")]
    public string Building { get; set; }
    
    [JsonProperty("apartment")]
    public string Apartment { get; set; }
    
    [JsonProperty("latitude")]
    public double Latitude { get; set; }
    
    [JsonProperty("longitude")]
    public double Longitude { get; set; }
}