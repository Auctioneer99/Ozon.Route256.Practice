using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class CancelResponse
{
    [JsonProperty("isSuccess")]
    public bool IsSuccess { get; set; }
    
    [JsonProperty("error")]
    public string Error { get; set; }
}