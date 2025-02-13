namespace CommentApp.Application.Comments.Queries.GetComments;

public class GetCommentsHandler(IApplicationDbContext dbContext)
    : IQueryHandler<GetCommentsQuery, GetCommentsResult>
{
    public async Task<GetCommentsResult> Handle(GetCommentsQuery query, CancellationToken cancellationToken)
    {
        var comments = await dbContext.Comments
            .AsNoTracking()
            .Include(f => f.File)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync(cancellationToken);

        return new GetCommentsResult(comments.ToCommentDtoList());
    }
}