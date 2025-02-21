namespace CommentApp.Infrastructure.Services.NotificationService;

public class NotificationService(SignalRService.SignalRService signalRService) : INotificationService
{
    public Task NotifyUsers(string message)
    {
        return signalRService.SendMessageToAll(message);
    }
}