namespace CommentApp.Domain.Events;

public record CommentCreatedEvent(
    CommentId CommentId,
    string UserName,
    string Email,
    string? HomePage,
    string Text,
    CommentId? ParentCommentId,
    FileId? FileId)
    : IDomainEvent;