using Academy.Catalog.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Academy.Catalog.Tests.Infrastructure;

/// <summary>
/// SQLite in-memory üzerinde gerçek bir CatalogDbContext. EF Core'un InMemory provider'ı LINQ→SQL
/// çevirisini gerçekten sınamadığından (bkz. plan riski) burada gerçek bir SQL motoru kullanılır —
/// N+1 ve sıralama/sayfalama hatalarını maskelemez. Her test kendi bağlantısını açıp kapatır,
/// testler arası veri sızıntısı olmaz.
/// </summary>
public sealed class CatalogSqliteFixture : IDisposable
{
    private readonly SqliteConnection _connection;

    public CatalogDbContext Context { get; }

    public CatalogSqliteFixture()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new CatalogDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
