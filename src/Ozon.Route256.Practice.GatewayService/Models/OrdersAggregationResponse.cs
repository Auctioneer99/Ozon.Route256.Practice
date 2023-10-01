using Newtonsoft.Json;

namespace Ozon.Route256.Practice.GatewayService.Models;

public class OrdersAggregationResponse
{
    [JsonProperty("aggregations")]
    public OrdersAggregationResponseEntry[] Aggregations { get; init; }

    public class OrdersAggregationResponseEntry
    {
        [JsonProperty("region")]
        public string Region { get; init; }
        
        [JsonProperty("ordersCount")]
        public int OrdersCount { get; init; }
        
        [JsonProperty("totalOrdersSum")]
        public double TotalOrdersSum { get; init; }
        
        [JsonProperty("totalOrdersWeight")]
        public double TotalOrdersWeight { get; init; }
        
        [JsonProperty("uniqueClientsCount")]
        public int UniqueClientsCount { get; init; }
    }
}