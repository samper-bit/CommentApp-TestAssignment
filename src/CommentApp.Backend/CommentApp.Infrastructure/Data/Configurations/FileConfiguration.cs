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
            commentId => commentId.Value,
            dbId => CommentId.Of(dbId));

        builder.Property(f => f.FilePath)
            .IsRequired()
            .HasMaxLength(300);
    }
}