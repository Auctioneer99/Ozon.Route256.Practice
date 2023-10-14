using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ozon.Route256.Practice.OrdersService.Kafka.Consumer.OrdersEvents.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum OrderState
{
    [EnumMember(Value = "Created")]
    Created = 1,
    [EnumMember(Value = "SentToCustomer")]
    SentToCustomer = 2,
    [EnumMember(Value = "Delivered")]
    Delivered = 3,
    [EnumMember(Value = "Lost")]
    Lost = 4,
    [EnumMember(Value = "Cancelled")]
    Cancelled = 5
}