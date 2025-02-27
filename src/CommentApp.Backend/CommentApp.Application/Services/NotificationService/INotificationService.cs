namespace CommentApp.Application.Services.NotificationService;

public interface INotificationService
{
    Task NotifyUsers(string message);
}