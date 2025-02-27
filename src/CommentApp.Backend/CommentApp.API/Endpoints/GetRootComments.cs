using CommentApp.Application.Comments.Queries.GetRootComments;

namespace CommentApp.API.Endpoints;

//public record GetRootCommentsRequest(PaginationRequest PaginationRequest);

public record GetRootCommentsResponse(PaginatedResult<CommentDto> Comments);

public class GetRootComments : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/comments/roots", async ([AsParameters] PaginationRequest request, ISender sender) =>
            {
                var result = await sender.Send(new GetRootCommentsQuery(request));

                var response = result.Adapt<GetRootCommentsResponse>();

                return Results.Ok(response);
            })
            .RequireCors("AllowAllOrigins")
            .WithName("GetRootComments")
            .RequireCors("AllowAllOrigins")
            .Produces<GetRootCommentsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Root Comments")
            .WithDescription("Get Root Comments");
    }
}