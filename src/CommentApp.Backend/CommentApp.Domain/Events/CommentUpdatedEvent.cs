namespace CommentApp.Domain.Events;

public record CommentUpdatedEvent(CommentId CommentId, string Text, FileId? FileId)
    : IDomainEvent;