namespace CommentApp.Application.Comments.Commands.CreateComment;

public record CreateCommentCommand(CreateCommentDto Comment, IFormFile? File) : ICommand<CreateCommentResult>;

public record CreateCommentResult(Guid Id);
public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.Comment.UserName).NotEmpty().WithMessage("UserName is required");
        RuleFor(x => x.Comment.Email).NotEmpty().WithMessage("Email is required");
        RuleFor(x => x.Comment.Text).NotEmpty().WithMessage("Text is required");
    }
}