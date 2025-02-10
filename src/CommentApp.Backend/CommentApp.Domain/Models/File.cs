namespace CommentApp.Domain.Models;
public class File : Entity<FileId>
{
    public CommentId? CommentId { get; private set; }

    public string FilePath { get; private set; } = default!;
    public FileType FileType { get; private set; }

    public static File Create(FileId id, CommentId? commentId, string filePath, FileType fileType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var file = new File
        {
            Id = id,
            CommentId = commentId,
            FilePath = filePath,
            FileType = fileType,
        };

        return file;
    }

    public void Update(string filePath, FileType fileType)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        FilePath = filePath;
        FileType = fileType;
    }
}