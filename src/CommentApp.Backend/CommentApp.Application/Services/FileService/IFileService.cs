namespace CommentApp.Application.Services.FileService;
public interface IFileService
{
    bool ValidateFile(IFormFile file);
    Task<bool> SaveFileAsync(IFormFile file, string path);
    bool DeleteFile(string path);
}