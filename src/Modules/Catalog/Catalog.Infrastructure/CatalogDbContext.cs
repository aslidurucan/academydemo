using Academy.Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Academy.Catalog.Infrastructure;

/// <summary>Catalog modülünün (M03) kendi şeması üzerindeki tek DbContext'i (bkz. docs/architecture.md).</summary>
public sealed class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
