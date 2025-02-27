namespace CommentApp.Application.Dtos;
public record CommentDto(
    Guid Id,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    Guid? ParentCommentId,
    List<CommentDto> ChildComments,
    FileDto? File,
    DateTime? CreatedAt,
    DateTime? LastModified);