using CommentApp.Application.Comments.Queries.GetComments;

namespace CommentApp.API.Endpoints;

//public record GetCommentsRequest();

public record GetCommentsResponse(IEnumerable<CommentDto> Comments);

public class GetComments : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/comments", async (ISender sender) =>
            {
                var result = await sender.Send(new GetCommentsQuery());

                var response = result.Adapt<GetCommentsResponse>();

                return Results.Ok(response);
            })
            .RequireCors("AllowAllOrigins")
            .WithName("GetComments")
            .Produces<GetCommentsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Comments")
            .WithDescription("Get Comments");
    }
}