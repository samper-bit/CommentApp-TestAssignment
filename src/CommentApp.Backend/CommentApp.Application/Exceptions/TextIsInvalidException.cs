namespace CommentApp.Application.Exceptions;
public class TextIsInvalidException(string message) : BadRequestException($"Comment not created. Reason: {message}");