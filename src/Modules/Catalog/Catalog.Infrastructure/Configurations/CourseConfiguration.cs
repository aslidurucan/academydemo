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

        // Listeleme sorgusunun sık kullandığı filtre/sıralama kombinasyonu için — p95 < 200ms
        // hedefini (spec 0001 §Constraints) destekler.
        builder.HasIndex(c => new { c.Status, c.CategoryId, c.PublishedAt });
    }
}
