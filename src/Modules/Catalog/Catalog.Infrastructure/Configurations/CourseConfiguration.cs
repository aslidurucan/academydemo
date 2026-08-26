using Academy.Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Academy.Catalog.Infrastructure.Configurations;

public sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        // Para alanı — her zaman decimal (bkz. AGENTS.md altın kural 3, docs/conventions.md §Para ve Yüzde).
        builder.Property(c => c.ListPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.CoverImageUrl)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(c => c.InstructorName)
            .IsRequired()
            .HasMaxLength(200);

        // Category, Course ile aynı şemada (catalog) yaşadığı için bu FK modül-içi bir referanstır;
        // docs/architecture.md §Yasak Liste'nin yasakladığı "şemalar arası FK/JOIN" bu değildir.
        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Kategori filtreli sorgu yolu için — p95 < 200ms hedefini (spec 0001 §Constraints) destekler.
        builder.HasIndex(c => new { c.Status, c.CategoryId, c.PublishedAt });

        // categoryId verilmeyen (muhtemelen en sık kullanılan) filtresiz tarama yolu için ayrı index —
        // (Status, CategoryId, PublishedAt) index'i CategoryId ile başladığından bu yolda global
        // PublishedAt DESC sıralaması için motor ek sort/merge yapmak zorunda kalabilirdi (QA bulgusu).
        // NOT: Bu index'in gerçekten kullanıldığı referans hacimde (10k kurs/50 kategori, gerçek
        // Postgres) EXPLAIN ANALYZE ile doğrulanmadı — bu ortamda çalışan bir Postgres yok; öneri
        // QA'nın işaret ettiği teorik riske karşı savunma amaçlı eklendi, doğrulama borç kalıyor.
        builder.HasIndex(c => new { c.Status, c.PublishedAt });
    }
}
