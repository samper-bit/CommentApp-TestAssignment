namespace CommentApp.Application.Comments.Commands.DeleteComment;

public record DeleteCommentCommand(Guid CommentId) : ICommand<DeleteCommentResult>;

public record DeleteCommentResult(bool IsSuccess);

public class DeleteCommentCommandValidator : AbstractValidator<DeleteCommentCommand>
{
    public DeleteCommentCommandValidator()
    {
        RuleFor(x => x.CommentId).NotEmpty().WithMessage("CommentId is required");
    }
}