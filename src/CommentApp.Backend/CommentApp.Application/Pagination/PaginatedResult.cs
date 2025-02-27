namespace CommentApp.Application.Pagination;
public class PaginatedResult<TEntity>(int pageIndex, int pageSize, long count, string sortField, bool ascending, IEnumerable<TEntity> data)
    where TEntity : class
{
    public int PageIndex { get; } = pageIndex;
    public int PageSize { get; } = pageSize;
    public long Count { get; } = count;
    public string SortField { get; } = sortField;
    public bool Ascending { get; } = ascending;
    public IEnumerable<TEntity> Data { get; } = data;
}