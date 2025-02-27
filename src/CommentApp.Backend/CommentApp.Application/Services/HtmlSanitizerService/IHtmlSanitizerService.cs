namespace CommentApp.Application.Services.HtmlSanitizerService;
public interface IHtmlSanitizerService
{
    string SanitizeTextWithTags(string html);
    string SanitizeString(string html);
}