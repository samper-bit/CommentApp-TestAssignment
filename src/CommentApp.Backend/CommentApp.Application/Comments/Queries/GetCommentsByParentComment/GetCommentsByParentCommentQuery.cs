namespace CommentApp.Application.Comments.Queries.GetCommentsByParentComment;
public record GetCommentsByParentCommentQuery(Guid ParentCommentId, PaginationRequest PaginationRequest)
    : IQuery<GetCommentsByParentCommentResult>;

public record GetCommentsByParentCommentResult(PaginatedResult<CommentDto> Comments);