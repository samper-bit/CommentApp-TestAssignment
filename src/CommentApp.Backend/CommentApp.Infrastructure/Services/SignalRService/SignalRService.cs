using CommentApp.Application.SignalR;

namespace CommentApp.Infrastructure.Services.SignalRService;

public class SignalRService
{
    private readonly IHubContext<CommentHub> _hubContext;

    public SignalRService(IHubContext<CommentHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendMessageToAll(string message)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
    }
}