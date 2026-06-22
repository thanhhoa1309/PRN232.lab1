namespace Prn232.Lab1.Service.Utils;

public static class ListApiHelper
{
    public static ApiResult ToListResponse<T>(PagedResult<T> result, string message, string? fields)
    {
        object items = string.IsNullOrWhiteSpace(fields)
            ? result.Items
            : FieldsHelper.SelectFields(result.Items, fields);

        var data = new ListResponseDto
        {
            Items = items,
            Pagination = result.Pagination
        };

        return ApiResult.SuccessResult(data, message);
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
