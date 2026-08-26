using Academy.Catalog.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Academy.Catalog.Api;

/// <summary>Catalog modülünün minimal API endpoint'leri (bkz. docs/conventions.md §Minimal API).</summary>
public static class CatalogEndpoints
{
    public static IEndpointRouteBuilder MapCatalogEndpoints(this IEndpointRouteBuilder app)
    {
        // Herkese açık: giriş yapmamış ziyaretçiler dahil (spec 0001 Requirements). Identity (M01)
        // henüz kurulmadığından bugün zaten bir auth zorunluluğu yok; ileride global bir "varsayılan
        // auth gerekli" politikası eklendiğinde bu endpoint'in sessizce kırılmaması için açıkça işaretli.
        app.MapGet("/api/courses", async (
                string? page,
                string? pageSize,
                string? categoryId,
                ICourseCatalogQueries queries,
                CancellationToken cancellationToken) =>
            {
                var query = CourseCatalogQuery.Create(page, pageSize, categoryId);
                var result = await queries.GetPublishedCoursesAsync(query, cancellationToken);

                var response = new CourseListResponse(
                    result.Items
                        .Select(item => new CourseListItemResponse(
                            item.Id,
                            item.Title,
                            item.ListPrice,
                            item.CategoryName,
                            item.InstructorName,
                            item.CoverImageUrl))
                        .ToList(),
                    result.TotalCount,
                    result.Page,
                    result.PageSize);

                return Results.Ok(response);
            })
            .WithName("GetPublishedCourses")
            .AllowAnonymous();

        return app;
    }
}
