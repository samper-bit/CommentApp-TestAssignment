namespace CommentApp.Application.Comments.Commands.DeleteComment;
public class DeleteCommentHandler(IApplicationDbContext dbContext)
    : ICommandHandler<DeleteCommentCommand, DeleteCommentResult>
{
    public async Task<DeleteCommentResult> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        var commentId = CommentId.Of(command.CommentId);
        var comment = await dbContext.Comments.FindAsync(commentId, cancellationToken);

        if (comment == null)
            throw new CommentNotFoundException(command.CommentId);

        dbContext.Comments.Remove(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteCommentResult(true);
    }
}