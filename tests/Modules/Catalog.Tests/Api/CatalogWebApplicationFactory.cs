using Academy.Catalog.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Academy.Catalog.Tests.Api;

/// <summary>
/// Gerçek Host'u (Program) ayağa kaldırır ama CatalogDbContext'i Npgsql yerine SQLite in-memory'e
/// bağlar — HTTP query-string binding'inin (ör. sayısal olmayan page/pageSize) gerçekten nasıl
/// davrandığını, Domain'e ulaşmadan önce ASP.NET Core'un ne yaptığını sınamak için.
/// </summary>
public sealed class CatalogWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public CatalogWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<CatalogDbContext>>();
            services.AddDbContext<CatalogDbContext>(options => options.UseSqlite(_connection));
        });
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await db.Database.EnsureCreatedAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
