namespace CommentApp.Application.Comments.EventHandlers;
public class CommentUpdatedEventHandler(ILogger<CommentUpdatedEventHandler> logger)
    : INotificationHandler<CommentUpdatedEvent>
{
    public Task Handle(CommentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}