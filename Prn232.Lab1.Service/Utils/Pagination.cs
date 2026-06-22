namespace Prn232.Lab1.Service.Utils;

public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}

public class ListResponseDto
{
    public object Items { get; set; } = Array.Empty<object>();
    public PaginationMetadata Pagination { get; set; } = new();
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public PaginationMetadata Pagination { get; set; } = new();

    public static PagedResult<T> Create(List<T> items, int totalItems, int page, int pageSize)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 10 : pageSize;

        return new PagedResult<T>
        {
            Items = items,
            Pagination = new PaginationMetadata
            {
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize)
            }
        };
    }
}
