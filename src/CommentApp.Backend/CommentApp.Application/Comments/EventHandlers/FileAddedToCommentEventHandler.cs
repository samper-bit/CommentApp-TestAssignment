namespace CommentApp.Application.Comments.EventHandlers;
public class FileAddedToCommentEventHandler(ILogger<FileAddedToCommentEventHandler> logger)
    : INotificationHandler<FileAddedToCommentEvent>
{
    public Task Handle(FileAddedToCommentEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}