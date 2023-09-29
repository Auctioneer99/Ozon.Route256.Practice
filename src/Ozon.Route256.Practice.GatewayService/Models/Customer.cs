using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class Customer
{
    [JsonProperty("id")]
    public int Id { get; set; }
    
    [JsonProperty("firstName")]
    public string FirstName { get; set; }
    
    [JsonProperty("lastName")]
    public string LastName { get; set; }
    
    [JsonProperty("mobileNumber")]
    public string MobileNumber { get; set; }
    
    [JsonProperty("email")]
    public string Email { get; set; }
    
    [JsonProperty("defaultAddress")]
    public Address DefaultAddress { get; set; }
    
    [JsonProperty("address")]
    public Address[] Address { get; set; }
}