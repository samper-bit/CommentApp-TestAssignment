namespace CommentApp.API.Endpoints;

public class GetFile : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/{fileName}", (string fileName) =>
            {
                var fileFullPath = Path.Combine("wwwroot", "files", fileName);

                if (!File.Exists(fileFullPath))
                {
                    return Results.NotFound("File not found!");
                }

                var fileBytes = File.ReadAllBytes(fileFullPath);

                var contentType = fileName.ToLower() switch
                {
                    var f when f.EndsWith(".jpg") => "image/jpeg",
                    var f when f.EndsWith(".jpeg") => "image/jpeg",
                    var f when f.EndsWith(".png") => "image/png",
                    var f when f.EndsWith(".gif") => "image/gif",
                    var f when f.EndsWith(".txt") => "text/plain",
                    _ => "application/octet-stream",
                };

                return Results.File(fileBytes, contentType, fileName);
            })
            .RequireCors("AllowAllOrigins")
            .WithName("GetFile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get File")
            .WithDescription("Get File");
    }
}