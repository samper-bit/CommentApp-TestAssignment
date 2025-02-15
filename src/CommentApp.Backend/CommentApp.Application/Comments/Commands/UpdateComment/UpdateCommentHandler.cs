namespace CommentApp.Application.Comments.Commands.UpdateComment;

public class UpdateCommentHandler
    (IApplicationDbContext dbContext, IHtmlSanitizerService sanitizer, IFileService fileService)
    : ICommandHandler<UpdateCommentCommand, UpdateCommentResult>
{
    public async Task<UpdateCommentResult> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        var sanitizedRequest = command with
        {
            Comment = command.Comment with
            {
                Text = sanitizer.SanitizeTextWithTags(command.Comment.Text),
            }
        };

        if (sanitizedRequest.Comment.Text != command.Comment.Text)
            throw new TextIsInvalidException("Invalid HTML tags detected!");

        if (command.File != null)
            fileService.ValidateFileAsync(command.File);

        var commentId = CommentId.Of(command.Comment.Id);
        var comment =
            await dbContext.Comments
                .AsNoTracking()
                .Include(c => c.File)
                .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

        if (comment == null)
            throw new CommentNotFoundException(command.Comment.Id);

        if (comment.File?.FilePath != null)
        {
            fileService.DeleteFile(comment.Id.Value.ToString());

            if (command.File == null)
            {
                dbContext.Files.Remove(comment.File);
                comment.RemoveFile();
            }
        }

        UpdateCommentWithNewValues(comment, command.Comment, command.File);

        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (command.File != null)
            await fileService.SaveFileAsync(command.File, comment.Id.Value.ToString());

        return new UpdateCommentResult(true);
    }

    private void UpdateCommentWithNewValues(Comment comment, UpdateCommentDto commentDto, IFormFile? file)
    {
        comment.Update(
            text: commentDto.Text,
            file: comment.File);

        if (file != null)
        {
            if (comment.File == null)
            {
                var fileId = comment.File != null ? comment.File.Id : FileId.Of(Guid.NewGuid());

                var fileDomain = File.Create(
                    id: fileId,
                    commentId: comment.Id,
                    fileExtension: Path.GetExtension(file.FileName));

                comment.AddFile(fileDomain);

                dbContext.Files.Add(fileDomain);
            }
            else
                comment.UpdateFile(Path.GetExtension(file.FileName));
        }
    }
}