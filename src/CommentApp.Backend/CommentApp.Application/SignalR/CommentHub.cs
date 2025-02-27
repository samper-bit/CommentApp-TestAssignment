namespace CommentApp.Application.SignalR;

public class CommentHub : Hub
{
    public async Task SendMessage()
    {
        await Clients.All.SendAsync("Message received");
    }
}