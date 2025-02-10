namespace CommentApp.Domain.Events;

public record ChildCommentRemovedEvent(CommentId ParentCommentId, CommentId ChildCommentId)
    : IDomainEvent;