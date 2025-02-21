using CommentApp.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace CommentApp.Infrastructure.Services.FileService;
public class FileService : IFileService
{
    private readonly string[] _imageTypes = [".jpg", ".jpeg", ".png", ".gif"];
    private const string FilesPath = "wwwroot/files";
    private const int MaxImageWidth = 320;
    private const int MaxImageHeight = 240;
    private const int MaxTextFileSize = 100 * 1024;

    public bool ValidateFileAsync(IFormFile file)
    {
        var fileType = GetFileType(file);
        if (String.IsNullOrWhiteSpace(fileType))
            throw new FileNotAddedException("Invalid file extension.");

        if (file.Length >= MaxTextFileSize && fileType == ".txt")
            throw new FileNotAddedException("File too big.");

        return true;
    }

    public async Task<bool> SaveFileAsync(IFormFile file, string commentId)
    {
        if (!Directory.Exists(FilesPath))
            Directory.CreateDirectory(FilesPath);

        var fileType = GetFileType(file);
        var fileName = $"{commentId}{fileType}";
        var filePath = Path.Combine(FilesPath, fileName);

        DeleteFile(commentId);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            if (_imageTypes.Contains(fileType))
            {
                var imageFile = await ResizeImage(file);

                await imageFile.CopyToAsync(stream);
                return true;
            }

            await file.CopyToAsync(stream);
        }

        return true;
    }

    public bool DeleteFile(string commentId)
    {
        var filePath = GetFilePath(commentId);

        if (String.IsNullOrWhiteSpace(filePath))
            return false;

        try
        {
            System.IO.File.Delete(filePath);
            return true;
        }
        catch (Exception ex)
        {
            throw new FileNotDeletedException(ex.Message);
        }
    }

    private static string GetFileType(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        return extension switch
        {
            ".jpg" => ".jpg",
            ".jpeg" => ".jpeg",
            ".png" => ".png",
            ".gif" => ".gif",
            ".txt" => ".txt",
            _ => ""
        };
    }

    private static async Task<IFormFile> ResizeImage(IFormFile file)
    {
        var fileType = GetFileType(file);
        await using var stream = file.OpenReadStream();

        using var image = await Image.LoadAsync(stream);

        int newWidth = image.Width, newHeight = image.Height;

        if (image.Width > MaxImageWidth || image.Height > MaxImageHeight)
        {
            var ratioX = (double)MaxImageWidth / image.Width;
            var ratioY = (double)MaxImageHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            newWidth = (int)(image.Width * ratio);
            newHeight = (int)(image.Height * ratio);
        }

        image.Mutate(x => x.Resize(newWidth, newHeight));

        var memoryStream = new MemoryStream();
        switch (fileType)
        {
            case ".jpeg" or ".jpg":
                await image.SaveAsJpegAsync(memoryStream);
                break;
            case ".png":
                await image.SaveAsPngAsync(memoryStream);
                break;
            case ".gif":
                await image.SaveAsGifAsync(memoryStream);
                break;
        }
        memoryStream.Position = 0;

        return new FormFile(memoryStream, 0, memoryStream.Length, file.Name, file.FileName)
        {
            Headers = file.Headers,
            ContentType = file.ContentType
        };
    }

    private static string GetFilePath(string commentId)
    {
        var allowedExtensions = new[] { ".jpeg", ".png", ".gif", ".txt" };

        foreach (var extension in allowedExtensions)
        {
            string filePath = Path.Combine(FilesPath, $"{commentId}{extension}");

            if (System.IO.File.Exists(filePath))
            {
                return filePath;
            }
        }

        return "";
    }
}