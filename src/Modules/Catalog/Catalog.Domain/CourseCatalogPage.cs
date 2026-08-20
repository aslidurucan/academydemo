namespace Academy.Catalog.Domain;

/// <summary>Bir sayfa katalog sonucu — öğeler + sayfalama meta verisi.</summary>
public sealed record CourseCatalogPage(
    IReadOnlyList<CourseListItem> Items,
    int TotalCount,
    int Page,
    int PageSize);
