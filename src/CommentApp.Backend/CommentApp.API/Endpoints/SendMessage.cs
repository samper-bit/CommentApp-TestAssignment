using CommentApp.Application.Services.NotificationService;
using CommentApp.Application.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace CommentApp.API.Endpoints;

public record SendMessageRequest(string Message);
public record SendMessageResponse(string Status);

public class SendMessage : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/send-message",
                async ([FromServices] INotificationService notificationService,
                    [FromServices] IHubContext<CommentHub> hubContext,
                    SendMessageRequest request) =>
                {
                    await notificationService.NotifyUsers(request.Message);

                    await hubContext.Clients.All.SendAsync("ReceiveMessage", request.Message);

                    return Results.Ok(new SendMessageResponse("Message Sent"));
                })
            .RequireCors("AllowAllOrigins")
            .WithName("SendMessage")
            .Produces<SendMessageResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Send Message")
            .WithDescription("Send message to all connected clients via SignalR");
    }
}