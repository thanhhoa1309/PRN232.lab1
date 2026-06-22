namespace Prn232.Lab1.Service.Utils;

public class ApiResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public object? Errors { get; set; }
    public PaginationMetadata? Pagination { get; set; }

    public static ApiResult SuccessResult(
        object? data,
        string message = "Request processed successfully.",
        PaginationMetadata? pagination = null)
    {
        return new ApiResult
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null,
            Pagination = pagination
        };
    }

    public static ApiResult FailureResult(string message, object? errors = null)
    {
        return new ApiResult
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors ?? new[] { message },
            Pagination = null
        };
    }
}

public class ApiResult<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }
    public PaginationMetadata? Pagination { get; set; }

    public static ApiResult<T> Ok(T data, string message = "Request processed successfully.")
    {
        return new ApiResult<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null,
            Pagination = null
        };
    }

    public static ApiResult<T> Failure(string message, object? errors = null)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Errors = errors ?? new[] { message },
            Pagination = null
        };
    }
}
