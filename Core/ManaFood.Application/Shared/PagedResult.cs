namespace ManaFood.Application.Shared;

public class PagedResult<T> : Paged<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class Paged<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int TotalCount { get; set; }
}