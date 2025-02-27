namespace CommentApp.Application.Comments.EventHandlers;

public class CommentCreatedEventHandler
(ILogger<CommentCreatedEventHandler> logger,
    IHubContext<CommentHub> hubContext)
    : INotificationHandler<CommentCreatedEvent>
{
    public async Task Handle(CommentCreatedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);

        await hubContext.Clients.All.SendAsync("ReceiveNewComment", cancellationToken);
    }
}