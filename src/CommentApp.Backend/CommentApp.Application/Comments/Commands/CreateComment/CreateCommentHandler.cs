namespace CommentApp.Application.Comments.Commands.CreateComment;

public class CreateCommentHandler(
    IApplicationDbContext dbContext,
    IHtmlSanitizerService sanitizer,
    IFileService fileService,
    ICacheService cacheService,
    IRabbitMqService rabbitMqService)
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

        if (sanitizedValues.Any(kv =>
                kv.Value != command.Comment.GetType().GetProperty(kv.Key)!.GetValue(command.Comment)?.ToString()))
            throw new TextIsInvalidException("Invalid HTML tags detected!");

        var comment = await CreateNewComment(command.Comment, command.File);
        await dbContext.Comments.AddAsync(comment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var cacheKey = comment.ParentCommentId == null ? "root-comments" : $"comments:{comment.ParentCommentId}";
        await cacheService.RemoveAsync(cacheKey);

        await rabbitMqService.PublishAsync(new
        {
            CommentId = comment.Id.Value,
            comment.UserName,
            comment.File,
            comment.Text,
            comment.CreatedAt,
        });

        return new CreateCommentResult(comment.Id.Value);
    }

    private async Task<Comment> CreateNewComment(CreateCommentDto commentDto, IFormFile? file)
    {
        var parentCommentId = commentDto.ParentCommentId.HasValue
            ? CommentId.Of(commentDto.ParentCommentId.Value)
            : null;

        var newCommentId = CommentId.Of(Guid.NewGuid());

        File? fileDomain = null;

        if (file != null)
        {
            fileService.ValidateFile(file);
            fileDomain = File.Create(
                id: FileId.Of(Guid.NewGuid()),
                commentId: newCommentId,
                fileExtension: Path.GetExtension(file.FileName));
            await fileService.SaveFileAsync(file, newCommentId.Value.ToString());
        }

        var newComment = Comment.Create(
            id: newCommentId,
            userName: commentDto.UserName,
            email: commentDto.Email,
            homePage: commentDto.HomePage,
            text: commentDto.Text,
            parentCommentId: parentCommentId,
            file: fileDomain);

        return newComment;
    }
}