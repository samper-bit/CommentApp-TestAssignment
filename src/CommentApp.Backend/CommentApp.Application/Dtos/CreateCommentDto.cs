namespace CommentApp.Application.Dtos;

public record CreateCommentDto(
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    Guid? ParentCommentId,
    CreateFileDto? File);