using Prn232.Lab1.Service.Utils;
using System.Text.Json;

namespace Prn232.Lab1.API.Middlewares;

// Catch all unhandled exceptions in the API
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Try to run the Controller
            await _next(context);
        }
        catch (Exception ex)
        {
            // If ANY error happens in Service or Controller -> catch here
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = ExceptionUtils.ExtractStatusCode(exception);
        context.Response.StatusCode = statusCode;

        var response = ApiResult<object>.Failure(
            statusCode.ToString(),
            exception.Message
        );

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

