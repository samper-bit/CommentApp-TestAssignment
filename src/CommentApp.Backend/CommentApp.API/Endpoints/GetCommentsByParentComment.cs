using CommentApp.Application.Comments.Queries.GetCommentsByParentComment;

namespace CommentApp.API.Endpoints;

//public record GetCommentsByParentCommentRequest(PaginationRequest<Guid> PaginationRequest);

public record GetCommentsByParentCommentResponse(PaginatedResult<CommentDto> Comments);

public class GetCommentsByParentComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/comments/parent/{parentId}",
                async (Guid parentId, [AsParameters] PaginationRequest request, ISender sender) =>
                {
                    var result = await sender.Send(new GetCommentsByParentCommentQuery(parentId, request));

                    var response = result.Adapt<GetCommentsByParentCommentResponse>();

                    return Results.Ok(response);
                })
            .RequireCors("AllowAllOrigins")
            .WithName("GetCommentsByParentComment")
            .Produces<GetCommentsByParentCommentResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithSummary("Get Comments By Parent Comment")
            .WithDescription("Get Comments By Parent Comment");
    }
}