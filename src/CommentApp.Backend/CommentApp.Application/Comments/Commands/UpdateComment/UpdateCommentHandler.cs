﻿namespace CommentApp.Application.Comments.Commands.UpdateComment;

public class UpdateCommentHandler
    (IApplicationDbContext dbContext,
        IHtmlSanitizerService sanitizer,
        IFileService fileService,
        ICacheService cacheService,
        IRabbitMqService rabbitMqService)
    : ICommandHandler<UpdateCommentCommand, UpdateCommentResult>
{
    public async Task<UpdateCommentResult> Handle(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        ValidateComment(command);

        var comment = await GetCommentFromDbAsync(command, cancellationToken);

        await UpdateCommentWithNewValues(comment, command.Comment, command.File);

        dbContext.Comments.Update(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        var cacheKey = comment.ParentCommentId == null ? "root-comments" : $"comments:{comment.ParentCommentId}";
        await cacheService.RemoveAsync(cacheKey);

        await rabbitMqService.PublishAsync(new
        {
            CommentId = comment.Id.Value,
            Action = "Updated",
            comment.UserName,
            ModifiedAt = DateTime.UtcNow,
            NewText = comment.Text,
            NewFile = comment.File
        });

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

    private void ValidateComment(UpdateCommentCommand command)
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
    }

    private async Task<Comment> GetCommentFromDbAsync(UpdateCommentCommand command, CancellationToken cancellationToken)
    {
        var commentId = CommentId.Of(command.Comment.Id);
        var comment =
            await dbContext.Comments
                .AsNoTracking()
        .Include(c => c.File)
                .FirstOrDefaultAsync(c => c.Id == commentId, cancellationToken);

        if (comment == null)
            throw new CommentNotFoundException(command.Comment.Id);

        return comment;
    }
}