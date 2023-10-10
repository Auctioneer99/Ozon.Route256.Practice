using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ozon.Route256.Practice.GatewayService.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum OrderFilterField
{
    [EnumMember(Value = "NoneField")]
    NoneField = 0,
    [EnumMember(Value = "Id")]
    Id = 1,
    [EnumMember(Value = "Count")]
    Count = 2,
    [EnumMember(Value = "TotalSum")]
    TotalSum = 3,
    [EnumMember(Value = "TotalWeight")]
    TotalWeight = 4,
    [EnumMember(Value = "OrderType")]
    OrderType = 5,
    [EnumMember(Value = "CreatedAt")]
    CreatedAt = 6,
    [EnumMember(Value = "OrderState")]
    OrderState = 7,
}