namespace CommentApp.Application.Comments.EventHandlers;
public class ChildCommentRemovedEventHandler(ILogger<ChildCommentRemovedEventHandler> logger)
    : INotificationHandler<ChildCommentRemovedEvent>
{
    public Task Handle(ChildCommentRemovedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}