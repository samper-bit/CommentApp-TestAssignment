using System.Drawing.Printing;

namespace CommentApp.Application.Comments.Queries.GetRootComments;

public class GetRootCommentsHandler(
    IApplicationDbContext dbContext,
    ICacheService cacheService)
    : IQueryHandler<GetRootCommentsQuery, GetRootCommentsResult>
{
    public async Task<GetRootCommentsResult> Handle(GetRootCommentsQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        var sortField = query.PaginationRequest.SortField;
        var ascending = query.PaginationRequest.Ascending;
        var cacheKey = "root-comments";

        if (IsDefaultPagination(query))
        {
            var cachedData = await cacheService.GetAsync(cacheKey);
            if (cachedData != null)
                return JsonSerializer.Deserialize<GetRootCommentsResult>(cachedData)!;
        }

        var comments = dbContext.Comments
            .AsNoTracking()
            .Include(c => c.File)
            .Where(c => c.ParentCommentId == null);

        comments = ascending
            ? comments.OrderBy(c => EF.Property<object>(c, sortField))
            : comments.OrderByDescending(c => EF.Property<object>(c, sortField));

        comments = comments
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .AsQueryable();

        var totalCount = await comments.LongCountAsync(cancellationToken);

        var result = new GetRootCommentsResult(
            new PaginatedResult<CommentDto>(
                pageIndex,
                pageSize,
                totalCount,
                sortField,
                ascending,
                comments.AsEnumerable().ToCommentDtoList()));

        if (IsDefaultPagination(query))
            await cacheService.SetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(5));

        return result;
    }

    private bool IsDefaultPagination(GetRootCommentsQuery query)
    {
        return query.PaginationRequest is { PageIndex: 0, PageSize: 25, SortField: "CreatedAt", Ascending: false };
    }
}