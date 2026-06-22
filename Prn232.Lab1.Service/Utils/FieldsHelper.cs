using System.Text.Json;
using System.Text.Json.Serialization;

namespace Prn232.Lab1.Service.Utils;

public static class FieldsHelper
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public static object? SelectFields(object? source, string? fields)
    {
        if (source == null)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(fields))
        {
            return source;
        }

        var fieldSet = fields
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var json = JsonSerializer.SerializeToElement(source, JsonOptions);
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var property in json.EnumerateObject())
        {
            if (!fieldSet.Contains(property.Name))
            {
                continue;
            }

            result[property.Name] = property.Value.ValueKind switch
            {
                JsonValueKind.String => property.Value.GetString(),
                JsonValueKind.Number => property.Value.TryGetInt64(out var longValue)
                    ? longValue
                    : property.Value.GetDouble(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                _ => JsonSerializer.Deserialize<object>(property.Value.GetRawText(), JsonOptions)
            };
        }

        return result;
    }

    public static List<object?> SelectFields<T>(IEnumerable<T> items, string? fields)
    {
        return items.Select(item => SelectFields(item, fields)).ToList();
    }
}
