using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class OrdersAggregationResponse
{
    [JsonProperty("aggregations")]
    public OrdersAggregationResponseEntry[] Aggregations { get; set; }

    public class OrdersAggregationResponseEntry
    {
        [JsonProperty("region")]
        public string Region { get; set; }
        
        [JsonProperty("ordersCount")]
        public int OrdersCount { get; set; }
        
        [JsonProperty("totalOrdersSum")]
        public double TotalOrdersSum { get; set; }
        
        [JsonProperty("totalOrdersWeight")]
        public double TotalOrdersWeight { get; set; }
        
        [JsonProperty("uniqueClientsCount")]
        public int UniqueClientsCount { get; set; }
    }
}