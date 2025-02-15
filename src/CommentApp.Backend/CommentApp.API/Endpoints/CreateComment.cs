using CommentApp.Application.Comments.Commands.CreateComment;

namespace CommentApp.API.Endpoints;

public class CreateCommentRequest
{
    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? HomePage { get; set; }
    public string Text { get; set; } = default!;
    public Guid? ParentCommentId { get; set; } = null;
    public IFormFile? File { get; set; } = null;
}

public record CreateCommentResponse(Guid Id);

public class CreateComment : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/comments", async ([FromForm] CreateCommentRequest request, ISender sender) =>
            {
                TypeAdapterConfig<CreateCommentRequest, CreateCommentCommand>
                    .NewConfig()
                    .Ignore(dest => dest.File!);

                var commentDto = request.Adapt<CreateCommentDto>();

                var command = new CreateCommentCommand(commentDto, request.File);

                var result = await sender.Send(command);

                var response = result.Adapt<CreateCommentResponse>();

                return Results.Created($"/comments/{response.Id}", response);
            })
            .DisableAntiforgery()
            .RequireCors("AllowAllOrigins")
            .WithName("CreateComment")
            .Produces<CreateCommentResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Create Comment")
            .WithDescription("Create Comment");
    }
}