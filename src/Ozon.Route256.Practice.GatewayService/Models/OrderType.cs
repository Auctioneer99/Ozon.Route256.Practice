using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ozon.Route256.Practice.GatewayService.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum OrderType 
{
    [EnumMember(Value = "UndefinedType")]
    UndefinedType = 0,
    [EnumMember(Value = "FirstType")]
    FirstType = 1,
    [EnumMember(Value = "SecondType")]
    SecondType = 2
}