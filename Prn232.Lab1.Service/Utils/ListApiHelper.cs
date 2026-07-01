namespace Prn232.Lab1.Service.Utils;

public static class ListApiHelper
{
    public static ApiResult ToListResponse<T>(PagedResult<T> result, string message, string? fields)
    {
        object data = string.IsNullOrWhiteSpace(fields)
            ? result.Items
            : FieldsHelper.SelectFields(result.Items, fields);

        return ApiResult.SuccessResult(data, message, result.Pagination);
    }

    public static int ResolvePageSize(int? size, int pageSize)
    {
        if (size.HasValue && size.Value > 0)
        {
            return size.Value;
        }

        return pageSize > 0 ? pageSize : 10;
    }
}
