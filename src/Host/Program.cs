using Academy.Catalog.Api;
using Academy.Catalog.Domain;
using Academy.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Catalog (M03) — bkz. docs/architecture.md §Şema Sabitleme.
builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CatalogDatabase")
        ?? throw new InvalidOperationException("ConnectionStrings:CatalogDatabase yapılandırılmamış.")));

builder.Services.AddScoped<ICourseCatalogQueries, CourseCatalogQueries>();

var app = builder.Build();

app.MapCatalogEndpoints();

app.Run();

// WebApplicationFactory<Program> entegrasyon testlerinin görebilmesi için (bkz. tests/Modules/Catalog.Tests).
public partial class Program
{
}
