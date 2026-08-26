using Academy.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Academy.Catalog.Infrastructure;

/// <summary>
/// <see cref="ICourseCatalogQueries"/>'in EF Core implementasyonu. Tek bir projeksiyon sorgusu +
/// bir sayım sorgusu kullanır; öğe başına ayrı bir sorgu yoktur (N+1 yasağı, bkz. docs/architecture.md).
/// </summary>
public sealed class CourseCatalogQueries : ICourseCatalogQueries
{
    private readonly CatalogDbContext _db;

    public CourseCatalogQueries(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<CourseCatalogPage> GetPublishedCoursesAsync(CourseCatalogQuery query, CancellationToken cancellationToken = default)
    {
        // AC-11: geçersiz formatlı categoryId, "kategoride kurs yok" ile aynı şekilde ele alınır —
        // filtre yokmuş gibi tüm kursları değil, doğrudan boş sonucu döner. DB'ye gitmeye gerek yok.
        if (query.HasInvalidCategoryFilter)
        {
            return new CourseCatalogPage(Array.Empty<CourseListItem>(), TotalCount: 0, query.Page, query.PageSize);
        }

        var filtered = _db.Courses
            .AsNoTracking()
            .Where(c => c.Status == PublicationStatus.Published);

        if (query.CategoryId is { } categoryId)
        {
            filtered = filtered.Where(c => c.CategoryId == categoryId);
        }

        var totalCount = await filtered.CountAsync(cancellationToken);

        var items = await filtered
            // PublishedAt DESC birincil sıralama (spec 0001 Requirements); Id ikincil sıralama
            // eşit PublishedAt değerlerinde bile sayfalar arası tutarlı (kayma/tekrarsız) sıra sağlar.
            .OrderByDescending(c => c.PublishedAt)
            .ThenByDescending(c => c.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Join(_db.Categories, c => c.CategoryId, cat => cat.Id, (c, cat) => new CourseListItem(
                c.Id,
                c.Title,
                c.ListPrice,
                cat.Name,
                c.InstructorName,
                c.CoverImageUrl))
            .ToListAsync(cancellationToken);

        return new CourseCatalogPage(items, totalCount, query.Page, query.PageSize);
    }
}
