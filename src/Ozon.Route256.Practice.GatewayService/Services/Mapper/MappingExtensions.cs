using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;

namespace Ozon.Route256.Practice.GatewayService.Services.Mapper;

public static class MappingExtensions
{
    public static T[] Map<T>(this RepeatedField<T> field)
    {
        return field.ToArray();
    }

    public static RepeatedField<T> ToRepeated<T>(this IEnumerable<T> values)
    {
        var result = new RepeatedField<T>();
        result.AddRange(values);
        return result;
    }
}