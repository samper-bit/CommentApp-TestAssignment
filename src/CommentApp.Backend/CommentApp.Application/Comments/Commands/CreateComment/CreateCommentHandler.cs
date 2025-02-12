namespace CommentApp.Application.Comments.Commands.CreateComment;

public class CreateCommentHandler(IApplicationDbContext dbContext)
    : ICommandHandler<CreateCommentCommand, CreateCommentResult>
{
    public async Task<CreateCommentResult> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        var comment = CreateNewComment(command.Comment);

        dbContext.Comments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateCommentResult(comment.Id.Value);
    }

    private Comment CreateNewComment(CreateCommentDto commentDto)
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

        if (commentDto.File != null)
        {
            var file = File.Create(
                id: FileId.Of(Guid.NewGuid()),
                commentId: newComment.Id,
                fileUploadName: commentDto.File.FileUploadName);

            newComment.AddFile(file);
        }

        return newComment;
    }
}