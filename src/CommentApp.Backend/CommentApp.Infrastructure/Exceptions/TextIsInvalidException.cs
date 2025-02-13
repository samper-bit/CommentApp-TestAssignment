namespace CommentApp.Infrastructure.Exceptions;
public class TextIsInvalidException(string message) : Exception($"Comment not created. Reason: {message}");