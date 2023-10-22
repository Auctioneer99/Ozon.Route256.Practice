using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ozon.Route256.Practice.GatewayService.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum OrderType 
{
    [EnumMember(Value = "Web")]
    Web = 1,
    [EnumMember(Value = "Mobile")]
    Mobile = 2,
    [EnumMember(Value = "Api")]
    Api = 3
}