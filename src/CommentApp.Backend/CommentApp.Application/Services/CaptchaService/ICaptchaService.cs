namespace CommentApp.Application.Services.CaptchaService;
public interface ICaptchaService
{
    (byte[] ImageData, string CaptchaId) GenerateCaptcha();
    bool ValidateCaptcha(string captchaId, string userInput);
}