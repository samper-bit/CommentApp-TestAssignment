namespace CommentApp.Infrastructure.Exceptions;
public class FileNotAddedException(string message) : Exception($"File can't be added. Reason: {message}");