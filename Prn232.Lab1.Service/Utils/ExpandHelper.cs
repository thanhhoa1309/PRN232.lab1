namespace Prn232.Lab1.Service.Utils;

public static class ExpandHelper
{
    public static bool HasExpand(string? expand, string name)
    {
        if (string.IsNullOrWhiteSpace(expand))
        {
            return false;
        }

        return expand
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Any(token => token.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
