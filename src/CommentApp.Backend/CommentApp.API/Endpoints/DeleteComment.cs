using CommentApp.Application.Comments.Commands.DeleteComment;

namespace CommentApp.API.Endpoints;

//public record DeleteCommentRequest(Guid Id);

public record DeleteCommentResponse(bool IsSuccess);

public class DeleteComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/comments/{id}", async (Guid Id, ISender sender) =>
            {
                var result = await sender.Send(new DeleteCommentCommand(Id));

                var response = result.Adapt<DeleteCommentResponse>();

                return Results.Ok(response);
            })
            .RequireCors("AllowAllOrigins")
            .WithName("DeleteComment")
            .Produces<DeleteCommentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Delete Comment")
            .WithDescription("Delete Comment");
    }
}