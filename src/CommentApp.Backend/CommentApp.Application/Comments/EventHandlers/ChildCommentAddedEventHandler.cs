namespace CommentApp.Application.Comments.EventHandlers;

public class ChildCommentAddedEventHandler(
    ILogger<CommentCreatedEventHandler> logger,
    IHubContext<CommentHub> hubContext)
    : INotificationHandler<ChildCommentAddedEvent>
{
    public async Task Handle(ChildCommentAddedEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("Domain Event handled: {DomainEvent}", notification.GetType().Name);

        await hubContext.Clients.All.SendAsync("ReceiveNewReply", notification, cancellationToken);
    }
}