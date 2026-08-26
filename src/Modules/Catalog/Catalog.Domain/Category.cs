namespace Academy.Catalog.Domain;

/// <summary>Katalog kategori taksonomisi. V1'de tek seviyeli (bkz. specs/0001-urun-listeleme.md).</summary>
public sealed class Category
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
