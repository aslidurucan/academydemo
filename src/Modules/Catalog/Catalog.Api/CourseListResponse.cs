namespace Academy.Catalog.Api;

public sealed record CourseListResponse(
    IReadOnlyList<CourseListItemResponse> Items,
    int TotalCount,
    int Page,
    int PageSize);
