namespace CommentApp.Infrastructure.Exceptions;
public class FileNotDeletedException(string message) : Exception($"File can't be deleted. Reason: {message}");