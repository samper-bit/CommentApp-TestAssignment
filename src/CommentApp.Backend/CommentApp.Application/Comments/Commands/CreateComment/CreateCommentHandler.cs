namespace CommentApp.Application.Comments.Commands.CreateComment;

public class CreateCommentHandler
    (IApplicationDbContext dbContext, IHtmlSanitizerService sanitizer, IFileService fileService)
    : ICommandHandler<CreateCommentCommand, CreateCommentResult>
{
    public async Task<CreateCommentResult> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        var sanitizedValues = new Dictionary<string, string>
        {
            { nameof(command.Comment.UserName), sanitizer.SanitizeString(command.Comment.UserName) },
            { nameof(command.Comment.Email), sanitizer.SanitizeString(command.Comment.Email) },
            { nameof(command.Comment.HomePage), sanitizer.SanitizeString(command.Comment.HomePage!) },
            { nameof(command.Comment.Text), sanitizer.SanitizeTextWithTags(command.Comment.Text) }
        };

        if (sanitizedValues.Any(kv => kv.Value != command.Comment.GetType().GetProperty(kv.Key)!.GetValue(command.Comment)?.ToString()))
            throw new TextIsInvalidException("Invalid HTML tags detected!");

        if (command.File != null)
            fileService.ValidateFileAsync(command.File);

        var comment = CreateNewComment(command.Comment, command.File);
        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        if (command.File != null)
            await fileService.SaveFileAsync(command.File, comment.Id.Value.ToString());

        return new CreateCommentResult(comment.Id.Value);
    }

    private Comment CreateNewComment(CreateCommentDto commentDto, IFormFile? file)
    {
        var parentCommentId = commentDto.ParentCommentId.HasValue
            ? CommentId.Of(commentDto.ParentCommentId.Value)
            : null;

        var newComment = Comment.Create(
            id: CommentId.Of(Guid.NewGuid()),
            userName: commentDto.UserName,
            email: commentDto.Email,
            homePage: commentDto.HomePage,
            text: commentDto.Text,
            parentCommentId: parentCommentId,
            file: null);

        if (file != null)
        {
            var fileDomain = File.Create(
                id: FileId.Of(Guid.NewGuid()),
                commentId: newComment.Id,
                fileExtension: Path.GetExtension(file.FileName));

            newComment.AddFile(fileDomain);
        }

        return newComment;
    }
}