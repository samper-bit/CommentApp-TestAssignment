namespace CommentApp.Infrastructure.Data.Configurations;
public class FileConfiguration : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id).HasConversion(
            fileId => fileId.Value,
            dbId => FileId.Of(dbId));

        builder.Property(c => c.CommentId).HasConversion(
            commentId => commentId == null ? (Guid?)null : commentId.Value,
            dbId => dbId == null ? null : CommentId.Of(dbId.Value));

        builder.Property(f => f.FilePath)
            .IsRequired()
            .HasMaxLength(300);
    }
}