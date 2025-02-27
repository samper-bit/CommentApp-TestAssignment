namespace CommentApp.Domain.Events;

public record FileAddedToCommentEvent(CommentId CommentId, FileId FileId, string FilePath)
    : IDomainEvent;