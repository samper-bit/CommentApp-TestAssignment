namespace CommentApp.Domain.ValueObjects;
public class CommentId
{
    public Guid Value { get; }
    private CommentId(Guid value) => Value = value;

    public static CommentId Of(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException("CommentId can not be empty.");
        }

        return new CommentId(value);
    }
}