namespace CommentApp.Domain.ValueObjects;
public class FileId
{
    public Guid Value { get; }
    private FileId(Guid value) => Value = value;

    public static FileId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("FileId can not be empty.");
        }

        return new FileId(value);
    }
}