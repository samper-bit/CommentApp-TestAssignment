using System.Reflection;

namespace CommentApp.Domain.Models;
public class File : Entity<FileId>
{
    public CommentId CommentId { get; private set; } = default!;

    public string FilePath { get; private set; } = default!;
    public FileType FileType { get; private set; }

    public static File Create(FileId id, CommentId commentId, string fileUploadName)
    {
        var file = new File
        {
            Id = id,
            CommentId = commentId,
            FilePath = GetFileType(commentId, fileUploadName).FilePath,
            FileType = GetFileType(commentId, fileUploadName).FileType,
        };

        return file;
    }

    public void Update(string fileUploadName)
    {
        FilePath = GetFileType(CommentId, fileUploadName).FilePath;
        FileType = GetFileType(CommentId, fileUploadName).FileType;
    }

    private static string SetFileTypeString(string fileName)
    {
        var fileTypeString = "";

        if (fileName.EndsWith(".jpg"))
            fileTypeString = ".jpg";
        else if (fileName.EndsWith(".png"))
            fileTypeString = ".png";
        else if (fileName.EndsWith(".gif"))
            fileTypeString = ".gif";
        else if (fileName.EndsWith(".txt"))
            fileTypeString = ".txt";

        if (String.IsNullOrWhiteSpace(fileTypeString))
            throw new DomainException("File type is not image or text");

        return fileTypeString;
    }

    private static FileType SetFileType(string fileTypeString)
    {
        FileType fileType = FileType.Image;

        if (fileTypeString == ".txt")
            fileType = FileType.Text;

        return fileType;
    }

    private static (string FilePath, FileType FileType) GetFileType(CommentId commentId, string fileName)
    {
        var filesPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Files");

        var fileTypeString = SetFileTypeString(fileName);

        var filePath = Path.Combine(filesPath, commentId.Value + fileTypeString);
        var fileType = SetFileType(fileTypeString);

        return (filePath, fileType);
    }
}