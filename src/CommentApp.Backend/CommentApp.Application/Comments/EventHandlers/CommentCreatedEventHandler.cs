namespace CommentApp.Application.Comments.EventHandlers;

public class CommentCreatedEventHandler(ILogger<CommentCreatedEventHandler> logger)
    : INotificationHandler<CommentCreatedEvent>
{
    public Task Handle(CommentCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}