namespace CommentApp.Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasConversion(
            commentId => commentId.Value,
            dbId => CommentId.Of(dbId));

        builder.Property(c => c.ParentCommentId).HasConversion(
            commentId => commentId!.Value,
            dbId => CommentId.Of(dbId));

        builder.Property(c => c.UserName)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(2000);

        builder.HasOne<Comment>()
            .WithMany(c => c.ChildComments)
            .HasForeignKey("ParentCommentId")
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.File)
            .WithOne()
            .HasForeignKey<File>(c => c.CommentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}