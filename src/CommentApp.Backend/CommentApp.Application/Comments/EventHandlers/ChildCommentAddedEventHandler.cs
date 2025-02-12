namespace CommentApp.Application.Comments.EventHandlers;
public class ChildCommentAddedEventHandler(ILogger<ChildCommentAddedEventHandler> logger)
    : INotificationHandler<ChildCommentAddedEvent>
{
    public Task Handle(ChildCommentAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}