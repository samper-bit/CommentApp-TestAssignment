namespace CommentApp.Application.Comments.Queries.GetComments;

public record GetCommentsQuery() : IQuery<GetCommentsResult>;

public record GetCommentsResult(IEnumerable<CommentDto> Comments);