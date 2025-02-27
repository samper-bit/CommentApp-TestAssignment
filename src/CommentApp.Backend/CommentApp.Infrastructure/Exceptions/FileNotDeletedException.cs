namespace CommentApp.Infrastructure.Exceptions;
public class FileNotDeletedException(string message) : BadRequestException($"File can't be deleted. Reason: {message}");