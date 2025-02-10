namespace CommentApp.Domain.Events;

public record ChildCommentAddedEvent(CommentId ParentCommentId, CommentId ChildCommentId)
    : IDomainEvent;