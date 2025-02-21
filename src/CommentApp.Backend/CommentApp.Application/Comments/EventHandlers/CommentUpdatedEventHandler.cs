namespace CommentApp.Application.Comments.EventHandlers;

public class CommentUpdatedEventHandler(
    ILogger<CommentCreatedEventHandler> logger,
    IHubContext<CommentHub> hubContext)
    : INotificationHandler<CommentUpdatedEvent>
{
    public async Task Handle(CommentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);

        await hubContext.Clients.All.SendAsync("ReceiveEditedComment", notification, cancellationToken);
    }
}