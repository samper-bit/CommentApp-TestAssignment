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

        var commentId = CommentId.Of(command.Comment.Id);
        var comment =
            await dbContext.Comments
                .AsNoTracking()
                .Include(c => c.File)
                .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

        if (comment == null)
            throw new CommentNotFoundException(command.Comment.Id);

        await UpdateCommentWithNewValues(comment, command.Comment, command.File);

        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateCommentResult(true);
    }

    private async Task UpdateCommentWithNewValues(Comment comment, UpdateCommentDto commentDto, IFormFile? file)
    {
        if (comment.File?.FilePath != null)
        {
            fileService.DeleteFile(comment.Id.Value.ToString());

            if (file == null)
            {
                dbContext.Files.Remove(comment.File);
                comment.RemoveFile();
            }
        }

        File? fileDomain = null;

        if (file != null)
        {
            fileService.ValidateFile(file);

            var fileId = comment.File != null ? comment.File.Id : FileId.Of(Guid.NewGuid());

            fileDomain = File.Create(
                id: fileId,
                commentId: comment.Id,
                fileExtension: Path.GetExtension(file.FileName));

            if (comment.File == null)
                await dbContext.Files.AddAsync(fileDomain);
            else
                dbContext.Files.Update(fileDomain);

            await fileService.SaveFileAsync(file, comment.Id.Value.ToString());
        }

        comment.Update(
            text: commentDto.Text,
            file: fileDomain);
    }
}