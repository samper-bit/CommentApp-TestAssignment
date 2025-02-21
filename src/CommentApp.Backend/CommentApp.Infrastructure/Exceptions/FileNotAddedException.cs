namespace CommentApp.Infrastructure.Exceptions;
public class FileNotAddedException(string message) : BadRequestException($"File can't be added. Reason: {message}");