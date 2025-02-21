namespace CommentApp.Application.Exceptions;
public class CommentNotFoundException(Guid id) : NotFoundException("Comment", id);