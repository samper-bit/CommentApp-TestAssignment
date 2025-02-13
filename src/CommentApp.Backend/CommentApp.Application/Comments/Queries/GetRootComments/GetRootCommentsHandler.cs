namespace CommentApp.Application.Comments.Queries.GetRootComments;

public class GetRootCommentsHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetRootCommentsQuery, GetRootCommentsResult>
{
    public async Task<GetRootCommentsResult> Handle(GetRootCommentsQuery query, CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        var sortField = query.PaginationRequest.SortField;
        var ascending = query.PaginationRequest.Ascending;

        var comments = dbContext.Comments
            .AsNoTracking()
            .Include(c => c.File)
            .Where(c => c.ParentCommentId == null)
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .AsQueryable();

        var totalCount = await comments.LongCountAsync(cancellationToken);

        comments = ascending
            ? comments.OrderBy(c => EF.Property<object>(c, sortField))
            : comments.OrderByDescending(c => EF.Property<object>(c, sortField));

        return new GetRootCommentsResult(
            new PaginatedResult<CommentDto>(
                pageIndex,
                pageSize,
                totalCount,
                sortField,
                ascending,
                comments.AsEnumerable().ToCommentDtoList()));
    }
}