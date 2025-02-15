namespace CommentApp.Application.Comments.Commands.DeleteComment;
public class DeleteCommentHandler
    (IApplicationDbContext dbContext, IFileService fileService)
    :ICommandHandler<DeleteCommentCommand, DeleteCommentResult>
{
    public async Task<DeleteCommentResult> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        var commentId = CommentId.Of(command.CommentId);
        var comment = await dbContext.Comments.FindAsync(commentId, cancellationToken);

        if (comment == null)
            throw new CommentNotFoundException(command.CommentId);

        dbContext.Comments.Remove(comment);
        await dbContext.SaveChangesAsync(cancellationToken);
        fileService.DeleteFile(commentId.Value.ToString() ?? string.Empty);

        return new DeleteCommentResult(true);
    }
}