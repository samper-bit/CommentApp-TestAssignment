using CommentApp.Infrastructure.Services.CaptchaService;

namespace CommentApp.API.Endpoints;

//public record GetCaptchaRequest();

public record GetCaptchaResponse(string CaptchaId);

public class GetCaptcha : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/captcha", ([FromServices] CaptchaService captchaService) =>
            {
                var captchaBytes = captchaService.GenerateCaptcha();
                var captchaId = captchaBytes.CaptchaId;
                var base64Image = Convert.ToBase64String(captchaBytes.ImageData);

                return Results.Ok(new
                {
                    captchaId,
                    imageUrl = $"data:image/png;base64,{base64Image}"
                });
            })
            .RequireCors("AllowAllOrigins")
            .WithName("GetCaptcha")
            .Produces<GetCaptchaResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Get Captcha")
            .WithDescription("Get Captcha");
    }
}