namespace CommentApp.Domain.Models;
public class Comment : Aggregate<CommentId>
{
    public string UserName { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public string? HomePage { get; private set; }
    public string Text { get; private set; } = default!;

    public CommentId? ParentCommentId { get; private set; }

    private readonly List<Comment> _childComments = new();
    public IReadOnlyList<Comment> ChildComments => _childComments.AsReadOnly();

    public File? File { get; private set; }

    public static Comment Create(CommentId id, string userName, string email, string? homePage, string text, CommentId? parentCommentId, File? file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userName);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var comment = new Comment
        {
            Id = id,
            UserName = userName,
            Email = email,
            HomePage = homePage,
            Text = text,
            ParentCommentId = parentCommentId,
            File = file,
        };

        comment.AddDomainEvent(new CommentCreatedEvent(id, userName, email, homePage, text, parentCommentId, file?.Id));

        if (file != null)
            comment.AddDomainEvent(new FileAddedToCommentEvent(id, file.Id, file.FilePath));

        return comment;
    }

    public void Update(string text, File? file)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        Text = text;
        if (file != null)
        {
            File = file;
        }

        AddDomainEvent(new CommentUpdatedEvent(Id, Text, file?.Id));
    }

    public void AddChildComment(Comment childComment)
    {
        childComment.ParentCommentId = Id;
        _childComments.Add(childComment);
        AddDomainEvent(new ChildCommentAddedEvent(Id, childComment.Id));
    }

    public void RemoveChildComment(CommentId childCommentId)
    {
        var childComment = _childComments.FirstOrDefault(x => x.Id == childCommentId);

        if (childComment != null)
        {
            _childComments.Remove(childComment);
            AddDomainEvent(new ChildCommentRemovedEvent(Id, childCommentId));
        }
    }

    public void AddFile(File file)
    {
        File = file;
        AddDomainEvent(new FileAddedToCommentEvent(Id, file.Id, file.FilePath));
    }

    public void UpdateFile(string fileUploadName)
    {
        File?.Update(fileUploadName);
    }
}