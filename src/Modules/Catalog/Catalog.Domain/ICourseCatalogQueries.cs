namespace Academy.Catalog.Domain;

/// <summary>
/// Katalog listeleme sorgusunun portu. Implementasyonu Catalog.Infrastructure'da (EF Core) —
/// bu arayüz Domain'de yaşar ki Api katmanı Infrastructure'a değil, yalnızca Domain'e bağımlı kalsın.
/// </summary>
public interface ICourseCatalogQueries
{
    Task<CourseCatalogPage> GetPublishedCoursesAsync(CourseCatalogQuery query, CancellationToken cancellationToken = default);
}
