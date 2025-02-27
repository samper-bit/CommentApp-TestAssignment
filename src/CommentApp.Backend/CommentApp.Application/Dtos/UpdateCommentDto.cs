namespace CommentApp.Application.Dtos;
public record UpdateCommentDto(
    Guid Id,
    string Text);