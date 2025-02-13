using System.Collections.Concurrent;
using System.Security.Cryptography;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace CommentApp.Infrastructure.Services.CaptchaService;

public class CaptchaService : ICaptchaService
{
    private readonly ConcurrentDictionary<string, string> _captchaStorage = new();

    public (byte[] ImageData, string CaptchaId) GenerateCaptcha()
    {
        var captchaText = GenerateRandomText();
        var captchaId = Guid.NewGuid().ToString();

        _captchaStorage[captchaId] = captchaText;

        var imageData = GenerateCaptchaImage(captchaText);
        return (imageData, captchaId);
    }

    public bool ValidateCaptcha(string captchaId, string userInput)
    {
        if (_captchaStorage.TryRemove(captchaId, out var storedValue))
        {
            return string.Equals(storedValue, userInput, StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static string GenerateRandomText()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return new string(Enumerable.Range(0, 6)
            .Select(_ => chars[RandomNumberGenerator.GetInt32(chars.Length)])
            .ToArray());
    }

    private static byte[] GenerateCaptchaImage(string captchaText)
    {
        int width = 200, height = 80;
        using var image = new Image<Rgba32>(width, height);

        var fontFamilies = SystemFonts.Collection.Families.ToList();
        var fontFamily = fontFamilies.FirstOrDefault(f => f.Name == "Arial");
        var font = new Font(fontFamily, 36, FontStyle.Bold);

        image.Mutate(ctx =>
        {
            ctx.Fill(Color.White);

            ctx.DrawText(captchaText, font, Color.Black, new PointF(20, 20));

            var random = new Random();
            for (int i = 0; i < 3; i++)
            {
                var points = new PointF[]
                {
                    new(random.Next(width), random.Next(height)),
                    new(random.Next(width), random.Next(height)),
                    new(random.Next(width), random.Next(height))
                };

                ctx.DrawPolygon(Color.Gray, 2, points);
            }

            for (int i = 0; i < 150; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                image[x, y] = Color.Gray;
            }
        });

        using var memoryStream = new MemoryStream();
        image.Save(memoryStream, new PngEncoder());
        return memoryStream.ToArray();
    }

    public bool VerifyCaptcha(string captchaId, string userInput)
    {
        if (_captchaStorage.TryGetValue(captchaId, out var correctText))
            return string.Equals(correctText, userInput, StringComparison.OrdinalIgnoreCase);

        return false;
    }

}