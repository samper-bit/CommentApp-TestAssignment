using CommentApp.Application.Comments.Commands.UpdateComment;

namespace CommentApp.API.Endpoints;

public class UpdateCommentRequest
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public IFormFile? File { get; set; } = null;
}

public record UpdateCommentResponse(bool IsSuccess);

public class UpdateComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("/comments", async ([FromForm] UpdateCommentRequest request, ISender sender) =>
            {
                TypeAdapterConfig<UpdateCommentRequest, UpdateCommentCommand>
                    .NewConfig()
                    .Ignore(dest => dest.File!);

                var commentDto = request.Adapt<UpdateCommentDto>();

                var command = new UpdateCommentCommand(commentDto, request.File);

                var result = await sender.Send(command);

                var response = result.Adapt<UpdateCommentResponse>();

                return Results.Ok(response);
            })
            .RequireCors("AllowAllOrigins")
            .DisableAntiforgery()
            .WithName("UpdateComment")
            .Produces<UpdateCommentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Update Comment")
            .WithDescription("Update Comment");
    }
}