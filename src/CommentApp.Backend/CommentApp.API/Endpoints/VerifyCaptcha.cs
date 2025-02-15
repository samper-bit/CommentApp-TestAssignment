using CommentApp.Infrastructure.Services.CaptchaService;

namespace CommentApp.API.Endpoints;

public record VerifyCaptchaRequest(string CaptchaId, string UserInput);
public record VerifyCaptchaResponse(bool IsValid);

public class VerifyCaptcha : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/verify-captcha", ([FromServices] CaptchaService captchaService, VerifyCaptchaRequest request) =>
            {
                var isValid = captchaService.VerifyCaptcha(request.CaptchaId, request.UserInput);

                if (isValid)
                    return Results.Ok(new VerifyCaptchaResponse(true));

                return Results.BadRequest(new VerifyCaptchaResponse(false));
            })
            .RequireCors("AllowAllOrigins")
            .WithName("VerifyCaptcha")
            .Produces<VerifyCaptchaResponse>(StatusCodes.Status200OK)
            .Produces<VerifyCaptchaResponse>(StatusCodes.Status400BadRequest)
            .WithSummary("Verify Captcha")
            .WithDescription("Verify Captcha");
    }
}