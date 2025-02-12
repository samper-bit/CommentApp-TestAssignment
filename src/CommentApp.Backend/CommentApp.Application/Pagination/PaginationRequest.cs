namespace CommentApp.Application.Pagination;

public record PaginationRequest(
    int PageIndex = 0,
    int PageSize = 25,
    string SortField = "CreatedAt",
    bool Ascending = false);