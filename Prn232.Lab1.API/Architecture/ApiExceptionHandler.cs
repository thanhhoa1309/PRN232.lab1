using Microsoft.AspNetCore.Diagnostics;
using Prn232.Lab1.Service.Utils;

namespace FUNewsManagementSystem.Architecture;

public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = ExceptionUtils.ExtractStatusCode(exception);
        var body = ExceptionUtils.CreateErrorResponse<object>(exception);

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(body, cancellationToken);
        return true;
    }
}
