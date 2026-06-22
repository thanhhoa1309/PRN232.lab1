namespace Prn232.Lab1.Service.Utils;

public static class ExceptionUtils
{
    public static int ExtractStatusCode(Exception ex)
    {
        if (ex.Data.Contains("StatusCode") && int.TryParse(ex.Data["StatusCode"]?.ToString(), out var code))
        {
            return code;
        }

        return 500;
    }

    public static string ExtractMessage(Exception ex)
    {
        return ex.Message ?? "An unexpected error occurred.";
    }

    public static ApiResult CreateErrorResponse(Exception ex)
    {
        var statusCode = ExtractStatusCode(ex);

        if (statusCode == 500)
        {
            return ApiResult.FailureResult("Internal server error", errors: null);
        }

        var message = ExtractMessage(ex);
        return ApiResult.FailureResult(message);
    }
}
