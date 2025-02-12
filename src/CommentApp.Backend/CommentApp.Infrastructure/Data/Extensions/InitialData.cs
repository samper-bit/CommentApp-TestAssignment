namespace CommentApp.Infrastructure.Data.Extensions;
internal class InitialData
{
    public static IEnumerable<Comment> CommentsWithAttachedFiles
    {
        get
        {
            // Creating comment1 with image file
            var comment1 = Comment.Create(CommentId.Of(new Guid("20e8daea-3f42-4c21-80f1-c6b7c7aaa542")), "John", "john@gmail.com", "https://john.homepage.com", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis blandit sem ullamcorper orci tincidunt, ut fringilla risus lacinia. Vivamus nec sodales elit. Nam malesuada enim in risus laoreet pellentesque.", null, null);
            var imageFile = File.Create(FileId.Of(new Guid("5334c996-8457-4cf0-815c-ed2b77c4ff61")), CommentId.Of(new Guid("20e8daea-3f42-4c21-80f1-c6b7c7aaa542")), "image.jpg");
            comment1.AddFile(imageFile);

            // Adding child comments to comment1
            var childComment1 = Comment.Create(CommentId.Of(new Guid("36f383f0-8fe1-4af9-bd94-234b26ff7d04")), "Mark", "mark@gmail.com", null, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis blandit sem ullamcorper orci tincidunt, ut fringilla risus lacinia. Vivamus nec sodales elit. Nam malesuada enim in risus laoreet pellentesque.", CommentId.Of(new Guid("20e8daea-3f42-4c21-80f1-c6b7c7aaa542")), null);
            var childComment2 = Comment.Create(CommentId.Of(new Guid("ee38e242-1a56-4a29-9d9b-2948948015aa")), "Josh", "josh@gmail.com", null, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis blandit sem ullamcorper orci tincidunt, ut fringilla risus lacinia. Vivamus nec sodales elit. Nam malesuada enim in risus laoreet pellentesque.", CommentId.Of(new Guid("20e8daea-3f42-4c21-80f1-c6b7c7aaa542")), null);
            comment1.AddChildComment(childComment1);
            comment1.AddChildComment(childComment2);

            // Adding child comments to childComment1
            var childCommentForChildComment1 = Comment.Create(CommentId.Of(new Guid("0a1cd315-116f-4ecf-8130-b5f208f98839")), "Michael", "michael@gmail.com", null, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis blandit sem ullamcorper orci tincidunt, ut fringilla risus lacinia. Vivamus nec sodales elit. Nam malesuada enim in risus laoreet pellentesque.", CommentId.Of(new Guid("36f383f0-8fe1-4af9-bd94-234b26ff7d04")), null);
            childComment1.AddChildComment(childCommentForChildComment1);

            // Creating comment2 with text file
            var comment2 = Comment.Create(CommentId.Of(new Guid("eec28f1d-8e6f-4b0d-bc69-aaf765db6975")), "Bob", "bob@gmail.com", "https://bob.homepage.com", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Duis blandit sem ullamcorper orci tincidunt, ut fringilla risus lacinia. Vivamus nec sodales elit. Nam malesuada enim in risus laoreet pellentesque.", null, null);
            var textFile = File.Create(FileId.Of(new Guid("c67d6323-e8b1-4bdf-9a75-b0d0d2e7e914")), CommentId.Of(new Guid("eec28f1d-8e6f-4b0d-bc69-aaf765db6975")), "text.txt");
            comment2.AddFile(textFile);

            return new List<Comment> { comment1, comment2, childComment1, childComment2, childCommentForChildComment1 };
        }
    }
}