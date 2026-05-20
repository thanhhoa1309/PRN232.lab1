namespace Prn232.Lab1.Service.Utils;

public static class FilterHelper
{
    public static Dictionary<string, string> Parse(string? filter)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(filter))
        {
            return result;
        }

        var pairs = filter.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var pair in pairs)
        {
            var parts = pair.Split(':', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
            {
                result[parts[0]] = parts[1];
            }
        }

        return result;
    }

    public static bool TryGetInt(Dictionary<string, string> filters, string key, out int value)
    {
        value = 0;
        return filters.TryGetValue(key, out var raw) && int.TryParse(raw, out value);
    }
}
