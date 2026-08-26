namespace Academy.Catalog.Domain;

/// <summary>
/// Katalogdaki bir kurs. V1'de tek kategoriye aittir (bkz. specs/0001-urun-listeleme.md).
/// </summary>
/// <remarks>
/// <see cref="InstructorName"/>, mimariye göre (docs/architecture.md Karar Günlüğü #8) Instructor
/// Management (M02) modülünden event ile beslenecek denormalize bir kopyadır. M02 henüz kurulmadığı
/// için bu alan spec 0001 kapsamında yalnızca okunur; event ile senkronize etme mekanizması ayrı bir
/// işin (M02 kurulduğunda açılacak bir spec'in) konusudur.
/// </remarks>
public sealed class Course
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required decimal ListPrice { get; init; }
    public required Guid CategoryId { get; init; }
    public required PublicationStatus Status { get; init; }

    /// <summary>
    /// Yalnızca <see cref="PublicationStatus.Published"/> durumundaki kurslarda dolu olur. UTC olarak
    /// saklanır (DateTimeOffset değil — SQLite EF Core sağlayıcısı DateTimeOffset üzerinde ORDER BY'ı
    /// desteklemiyor; testlerde gerçek bir SQL motoru kullanma kararının (bkz. plan) ortaya çıkardığı
    /// bir kısıt, tüm sağlayıcılarda tutarlı çalışsın diye DateTime UTC tercih edildi).
    /// </summary>
    public DateTime? PublishedAt { get; init; }

    public required string CoverImageUrl { get; init; }

    /// <summary>Zayıf referans — Instructor Management (M02) modülünün sahip olduğu kimlik.</summary>
    public required Guid InstructorId { get; init; }

    public required string InstructorName { get; init; }
}
