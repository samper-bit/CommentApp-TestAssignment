namespace CommentApp.Application.Exceptions;
public class CommentNotFoundException : Exception
{
    public CommentNotFoundException(string message) : base(message)
    {
    }

    public CommentNotFoundException(object key) : base($"Entity Comment ({key}) was not found.")
    {
    }
}