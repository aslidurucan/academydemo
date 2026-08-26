namespace Academy.Catalog.Domain;

/// <summary>Katalog listesindeki tek bir satır — AC gereği zorunlu asgari alan seti (spec 0001).</summary>
public sealed record CourseListItem(
    Guid Id,
    string Title,
    decimal ListPrice,
    string CategoryName,
    string InstructorName,
    string CoverImageUrl);
