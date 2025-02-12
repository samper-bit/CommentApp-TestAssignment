using Microsoft.EntityFrameworkCore;

namespace CommentApp.Application.Comments.Commands.UpdateComment;
public class UpdateCommentHandler(IApplicationDbContext dbContext)
    : ICommandHandler<UpdateCommentCommand, UpdateCommentResult>
{
    public async Task<UpdateCommentResult> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        var commentId = CommentId.Of(command.Comment.Id);
        var comment =
            await dbContext.Comments
                .AsNoTracking()
                .Include(c => c.File)
                .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

        if (comment == null)
            throw new CommentNotFoundException(command.Comment.Id);

        UpdateCommentWithNewValues(comment, command.Comment);

        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateCommentResult(true);
    }

    private void UpdateCommentWithNewValues(Comment comment, UpdateCommentDto commentDto)
    {
        comment.Update(
            text: commentDto.Text,
            file: comment.File);

        if (commentDto.File != null)
        {
            if (comment.File == null)
            {
                var file = File.Create(
                    id: FileId.Of(Guid.NewGuid()),
                    commentId: comment.Id,
                    fileUploadName: commentDto.File.FileUploadName);

                comment.AddFile(file);
            }
            else
                comment.File.Update(commentDto.File.FileUploadName);
        }
    }
}