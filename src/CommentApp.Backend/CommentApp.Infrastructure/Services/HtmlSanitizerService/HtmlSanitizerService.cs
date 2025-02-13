using Ganss.Xss;

namespace CommentApp.Infrastructure.Services.HtmlSanitizerService;
public class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlSanitizerService()
    {
        _sanitizer = new HtmlSanitizer();

        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedAttributes.Clear();
    }

    public string SanitizeTextWithTags(string html)
    {
        var sanitizer = _sanitizer;

        sanitizer.AllowedTags.Clear();
        sanitizer.AllowedTags.Add("a");
        sanitizer.AllowedTags.Add("code");
        sanitizer.AllowedTags.Add("i");
        sanitizer.AllowedTags.Add("strong");

        sanitizer.AllowedAttributes.Clear();

        sanitizer.AllowedAttributes.Add("href");
        sanitizer.AllowedAttributes.Add("title");

        return sanitizer.Sanitize(html);
    }

    public string SanitizeString(string html)
    {
        return _sanitizer.Sanitize(html);
    }
}