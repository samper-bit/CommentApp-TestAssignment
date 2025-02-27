namespace CommentApp.Domain.Models;
public class File : Entity<FileId>
{
    public CommentId CommentId { get; private init; } = default!;

    public string FilePath { get; private set; } = default!;
    public FileType FileType { get; private set; }

    public static File Create(FileId id, CommentId commentId, string fileExtension)
    {
        var file = new File
        {
            Id = id,
            CommentId = commentId,
            FilePath = GetFileType(commentId, fileExtension).FilePath,
            FileType = GetFileType(commentId, fileExtension).FileType,
        };

        return file;
    }

    public void Update(string fileExtension)
    {
        FilePath = GetFileType(CommentId, fileExtension).FilePath;
        FileType = GetFileType(CommentId, fileExtension).FileType;
    }

    private static FileType SetFileType(string fileExtension)
    {
        FileType fileType = FileType.Image;

        if (fileExtension == ".txt")
            fileType = FileType.Text;

        return fileType;
    }

    private static (string FilePath, FileType FileType) GetFileType(CommentId commentId, string fileExtension)
    {
        var filePath = Path.Combine(commentId.Value + fileExtension);
        var fileType = SetFileType(fileExtension);

        return (filePath, fileType);
    }
}