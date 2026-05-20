using System.Reflection;
using System.Text.Json;

namespace Prn232.Lab1.Service.Utils;

public static class ResourceHelper
{
    public static string ReadResource(string relativePath, Assembly fromAssembly)
    {
        var assembly = fromAssembly ?? typeof(ResourceHelper).Assembly;
        var str = relativePath.Replace('/', '.').Replace('\\', '.');

        using var manifestResourceStream = assembly.GetManifestResourceStream(assembly.GetName().Name + "." + str);
        if (manifestResourceStream == null)
            throw new IOException("Failed to read manifest resource.");
        using var streamReader = new StreamReader(manifestResourceStream);
        return streamReader.ReadToEnd();
    }

    public static string ReadJsonResource(
        string relativePath,
        Assembly fromAssembly,
        bool stripWhitespace = false)
    {
        return !stripWhitespace
            ? ReadResource(relativePath, fromAssembly)
            : StripJsonWhitespace(ReadResource(relativePath, fromAssembly));
    }

    private static string StripJsonWhitespace(string json)
    {
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document.RootElement);
    }

    public static int DateTimeValidate(DateTime startDate, DateTime endDate)
    {
        // Only consider date part, ignore time
        startDate = startDate.Date;
        endDate = endDate.Date;

        if (endDate < startDate)
            throw ErrorHelper.BadRequest("EndDate cannot be earlier than StartDate.");

        return (endDate - startDate).Days + 1;
    }
}
