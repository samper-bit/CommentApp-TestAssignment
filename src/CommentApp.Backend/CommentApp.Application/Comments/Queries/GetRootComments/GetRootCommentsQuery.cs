namespace CommentApp.Application.Comments.Queries.GetRootComments;

public record GetRootCommentsQuery(PaginationRequest PaginationRequest) : IQuery<GetRootCommentsResult>;

public record GetRootCommentsResult(PaginatedResult<CommentDto> Comments);