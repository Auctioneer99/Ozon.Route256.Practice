using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public sealed class Customer
{
    [JsonProperty("id")]
    public int Id { get; init; }
    
    [JsonProperty("firstName")]
    public string FirstName { get; init; }
    
    [JsonProperty("lastName")]
    public string LastName { get; init; }
    
    [JsonProperty("mobileNumber")]
    public string MobileNumber { get; init; }
    
    [JsonProperty("email")]
    public string Email { get; init; }
    
    [JsonProperty("defaultAddress")]
    public Address DefaultAddress { get; init; }
    
    [JsonProperty("address")]
    public Address[] Address { get; init; }
}