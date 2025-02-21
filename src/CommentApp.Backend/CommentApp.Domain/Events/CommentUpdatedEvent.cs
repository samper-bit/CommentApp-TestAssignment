namespace CommentApp.Domain.Events;

public record CommentUpdatedEvent(CommentId CommentId, string Text, File? File)
    : IDomainEvent;