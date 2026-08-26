namespace Academy.Catalog.Api;

public sealed record CourseListItemResponse(
    Guid Id,
    string Title,
    decimal ListPrice,
    string CategoryName,
    string InstructorName,
    string CoverImageUrl);
