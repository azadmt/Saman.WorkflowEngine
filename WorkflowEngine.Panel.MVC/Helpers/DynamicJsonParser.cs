using System.Dynamic;
using System.Text.Json;

public static class DynamicJsonParser
{
    public static IEnumerable<object> ParseToObjects(object value)
    {
        if (value == null)
            return Enumerable.Empty<object>();

        // اگر از API آمده باشد → JsonElement
        if (value is JsonElement je)
        {
            if (je.ValueKind == JsonValueKind.Array)
                return ParseArray(je);

            if (je.ValueKind == JsonValueKind.Object)
                return new List<object> { ParseObject(je) };
        }

        // اگر از قبل لیست باشد
        if (value is IEnumerable<object> list)
            return list;

        return new List<object> { value };
    }

    private static List<object> ParseArray(JsonElement array)
    {
        var result = new List<object>();

        foreach (var item in array.EnumerateArray())
        {
            if (item.ValueKind == JsonValueKind.Object)
                result.Add(ParseObject(item));
            else
                result.Add(GetPrimitiveValue(item));
        }

        return result;
    }

    private static ExpandoObject ParseObject(JsonElement obj)
    {
        IDictionary<string, object?> expando = new ExpandoObject();

        foreach (var prop in obj.EnumerateObject())
        {
            expando[prop.Name] = prop.Value.ValueKind switch
            {
                JsonValueKind.String => prop.Value.GetString(),
                JsonValueKind.Number => prop.Value.TryGetInt64(out var l) ? l : prop.Value.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Array => ParseArray(prop.Value),
                JsonValueKind.Object => ParseObject(prop.Value),
                _ => prop.Value.ToString()
            };
        }

        return (ExpandoObject)expando;
    }

    private static object? GetPrimitiveValue(JsonElement el)
    {
        return el.ValueKind switch
        {
            JsonValueKind.String => el.GetString(),
            JsonValueKind.Number => el.TryGetInt64(out var l) ? l : el.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => el.ToString()
        };
    }
}