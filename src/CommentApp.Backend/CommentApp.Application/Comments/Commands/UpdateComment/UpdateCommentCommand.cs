namespace CommentApp.Application.Comments.Commands.UpdateComment;

public record UpdateCommentCommand(UpdateCommentDto Comment) : ICommand<UpdateCommentResult>;

public record UpdateCommentResult(bool IsSuccess);

public class UpdateCommentCommandValidator : AbstractValidator<UpdateCommentCommand>
{
    public UpdateCommentCommandValidator()
    {
        RuleFor(x => x.Comment.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Comment.Text).NotEmpty().WithMessage("Text is required");
    }
}