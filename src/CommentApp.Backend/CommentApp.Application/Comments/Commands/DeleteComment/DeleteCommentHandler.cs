﻿namespace CommentApp.Application.Comments.Commands.DeleteComment;

public class DeleteCommentHandler(
    IApplicationDbContext dbContext,
    IFileService fileService,
    ICacheService cacheService,
    IRabbitMqService rabbitMqService)
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
        fileService.DeleteFile(commentId.Value.ToString() ?? string.Empty);

        var cacheKey = comment.ParentCommentId == null ? "root-comments" : $"comments:{comment.ParentCommentId}";
        await cacheService.RemoveAsync(cacheKey);

        await rabbitMqService.PublishAsync(new
        {
            CommentId = comment.Id.Value,
            Action = "Deleted",
            comment.UserName,
            DeletedAt = DateTime.UtcNow
        });

        return new DeleteCommentResult(true);
    }
}