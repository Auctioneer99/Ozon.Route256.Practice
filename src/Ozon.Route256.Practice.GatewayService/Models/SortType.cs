using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Ozon.Route256.Practice.GatewayService.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum SortType {
    [EnumMember(Value = "None")]
    None = 0,
    [EnumMember(Value = "Ascending")]
    Ascending = 1,
    [EnumMember(Value = "Descending")]
    Descending = 2
}