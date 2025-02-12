using Microsoft.EntityFrameworkCore;

namespace CommentApp.Application.Data;
public interface IApplicationDbContext
{
    DbSet<Comment> Comments { get; }
    DbSet<File> Files { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}