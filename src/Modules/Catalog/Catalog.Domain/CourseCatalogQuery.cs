namespace Academy.Catalog.Domain;

/// <summary>
/// Katalog listeleme isteğinin normalize edilmiş hâli. Ham (ve potansiyel olarak geçersiz)
/// page/pageSize/categoryId girdilerini spec 0001 kurallarına göre her zaman geçerli bir sorguya
/// çevirir — bu tür hiçbir zaman geçersiz bir durumda kurulamaz, bu yüzden endpoint tarafında
/// ayrıca bir doğrulama/reddetme adımına gerek kalmaz.
/// </summary>
public sealed class CourseCatalogQuery
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    /// <summary>
    /// page için güvenlik üst sınırı. CourseCatalogQueries, Skip((Page-1) * PageSize) hesabını int32
    /// aritmetiğiyle yapıyor; PageSize en fazla <see cref="MaxPageSize"/> olduğundan bu sınır,
    /// (MaxPage-1)*MaxPageSize çarpımının int.MaxValue'yu asla aşmamasını garanti eder (QA bulgusu:
    /// page=int.MaxValue gibi çok büyük ama biçimsel olarak geçerli bir sayı, taşıp negatife
    /// sarılan bir OFFSET'e dönüşüyordu). AC-10 zaten "aralık dışı sayfa → boş liste" bekliyor;
    /// bu sınır o davranışı bozmadan taşmayı engelliyor.
    /// </summary>
    public const int MaxPage = int.MaxValue / MaxPageSize;

    public int Page { get; }
    public int PageSize { get; }

    /// <summary>Geçerli formatta bir categoryId verildiyse o kategoriye daraltır; null ise filtre yoktur.</summary>
    public Guid? CategoryId { get; }

    /// <summary>
    /// categoryId verildi ama GUID formatında değildi. Bu, "categoryId hiç verilmedi" (filtre yok,
    /// tüm kurslar) durumundan kasıtlı olarak ayrıdır — AC-11 gereği sonuç her zaman boş olmalıdır.
    /// </summary>
    public bool HasInvalidCategoryFilter { get; }

    private CourseCatalogQuery(int page, int pageSize, Guid? categoryId, bool hasInvalidCategoryFilter)
    {
        Page = page;
        PageSize = pageSize;
        CategoryId = categoryId;
        HasInvalidCategoryFilter = hasInvalidCategoryFilter;
    }

    /// <summary>
    /// page ve pageSize birbirinden bağımsız doğrulanır (spec 0001, netleştirme 2026-08-21):
    /// biri geçersizse yalnızca o alan varsayılana döner, diğeri kullanıcının verdiği geçerli
    /// değerde kalır. pageSize üst sınırı aşarsa varsayılana değil, üst sınıra (100) sabitlenir.
    /// page de aynı şekilde <see cref="MaxPage"/>'e sabitlenir — bu, spec'in bir davranışı değil,
    /// saf bir taşma güvenliği önlemidir (bkz. MaxPage dokümantasyonu); gerçek kullanımda bu sınıra
    /// hiçbir zaman ulaşılmaz, aralık dışı sayfalar zaten AC-10 gereği boş liste döner.
    /// </summary>
    /// <remarks>
    /// page/pageSize ham string olarak alınır (int? değil): ASP.NET Core minimal API'nin opsiyonel
    /// bir <c>int?</c> parametresine sayısal olmayan bir değer verildiğinde sessizce null'a değil,
    /// 400 Bad Request'e düşen davranışı (bkz. Catalog.Api.CatalogEndpointsTests) burada bilerek
    /// devre dışı bırakılıyor — "istek reddedilmez" AC'lerini (AC-07, AC-08) sağlamak için parse işi
    /// tamamen Domain'e taşındı.
    /// </remarks>
    public static CourseCatalogQuery Create(string? page, string? pageSize, string? categoryId)
    {
        var normalizedPage = int.TryParse(page, out var parsedPage) && parsedPage > 0
            ? Math.Min(parsedPage, MaxPage)
            : DefaultPage;

        var normalizedPageSize = int.TryParse(pageSize, out var parsedPageSize) && parsedPageSize > 0
            ? Math.Min(parsedPageSize, MaxPageSize)
            : DefaultPageSize;

        var (parsedCategoryId, isInvalid) = ParseCategoryId(categoryId);

        return new CourseCatalogQuery(normalizedPage, normalizedPageSize, parsedCategoryId, isInvalid);
    }

    private static (Guid? CategoryId, bool IsInvalid) ParseCategoryId(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return (null, false);
        }

        return Guid.TryParse(raw, out var parsed) ? (parsed, false) : (null, true);
    }
}
