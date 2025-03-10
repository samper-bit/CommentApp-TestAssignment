﻿using CommentApp.Application.Comments.Queries.GetRootComments;

namespace CommentApp.Application.Comments.Queries.GetCommentsByParentComment;

public class GetCommentsByParentCommentHandler(
    IApplicationDbContext dbContext,
    ICacheService cacheService)
    : IQueryHandler<GetCommentsByParentCommentQuery, GetCommentsByParentCommentResult>
{
    public async Task<GetCommentsByParentCommentResult> Handle(GetCommentsByParentCommentQuery query,
        CancellationToken cancellationToken)
    {
        var pageIndex = query.PaginationRequest.PageIndex;
        var pageSize = query.PaginationRequest.PageSize;
        var sortField = query.PaginationRequest.SortField;
        var ascending = query.PaginationRequest.Ascending;
        var cacheKey = $"comments:{query.ParentCommentId}";

        if (IsDefaultPagination(query))
        {
            var cachedData = await cacheService.GetAsync(cacheKey);
            if (cachedData != null)
                return JsonSerializer.Deserialize<GetCommentsByParentCommentResult>(cachedData)!;
        }

        var comments = dbContext.Comments
            .AsNoTracking()
            .Include(c => c.File)
            .Where(c => c.ParentCommentId == CommentId.Of(query.ParentCommentId));

        var totalCount = await comments.LongCountAsync(cancellationToken);

        comments = ascending
            ? comments.OrderBy(c => EF.Property<object>(c, sortField))
            : comments.OrderByDescending(c => EF.Property<object>(c, sortField));

        comments = comments
            .Skip(pageSize * pageIndex)
            .Take(pageSize)
            .AsQueryable();

        var result = new GetCommentsByParentCommentResult(
            new PaginatedResult<CommentDto>(
                pageIndex,
                pageSize,
                totalCount,
                sortField,
                ascending,
                comments.AsEnumerable().ToCommentDtoList()));

        if (IsDefaultPagination(query))
            await cacheService.SetAsync(cacheKey, JsonSerializer.Serialize(result), TimeSpan.FromMinutes(5));

        return result;
    }

    private bool IsDefaultPagination(GetCommentsByParentCommentQuery query)
    {
        return query.PaginationRequest is { PageIndex: 0, PageSize: 25, SortField: "CreatedAt", Ascending: false };
    }
}