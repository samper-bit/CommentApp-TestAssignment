namespace CommentApp.Application.Extensions;
public static class CommentExtension
{
    public static IEnumerable<CommentDto> ToCommentDtoList(this IEnumerable<Comment>? comments)
    {
        var commentMap = new Dictionary<Guid, CommentDto>();

        CommentDto MapComment(Comment comment)
        {
            if (commentMap.TryGetValue(comment.Id.Value, out var existingDto))
                return existingDto;

            var dto = new CommentDto(
                Id: comment.Id.Value,
                UserName: comment.UserName,
                Email: comment.Email,
                HomePage: comment.HomePage,
                Text: comment.Text,
                ParentCommentId: comment.ParentCommentId?.Value,
                ChildComments: new List<CommentDto>(),
                File: comment.File != null ? new FileDto(comment.File.FilePath, comment.File.FileType) : null
            );

            commentMap[comment.Id.Value] = dto;

            return dto;
        }

        if (comments != null)
        {
            comments = comments.ToList();
            var result = comments.Select(MapComment).ToList();

            foreach (var comment in comments)
            {
                if (comment.ParentCommentId != null &&
                    commentMap.TryGetValue(comment.ParentCommentId.Value, out var parentDto))
                {
                    var childDto = commentMap[comment.Id.Value];
                    parentDto.ChildComments.Add(childDto);
                }
            }

            return result;
        }

        return new List<CommentDto>();
    }
}