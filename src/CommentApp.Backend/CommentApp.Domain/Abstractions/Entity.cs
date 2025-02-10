namespace CommentApp.Domain.Abstractions;
public class Entity<T> : IEntity<T>
{
    public T Id { get; set; } = default!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? LastModified { get; set; }
}