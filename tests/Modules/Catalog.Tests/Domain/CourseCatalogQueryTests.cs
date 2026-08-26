using Academy.Catalog.Domain;

namespace Academy.Catalog.Tests.Domain;

/// <summary>
/// CourseCatalogQuery.Create normalizasyon kuralları — saf, DB'siz. Spec 0001 AC-01, AC-03,
/// AC-07, AC-08, AC-09 ve AC-11'in "geçersiz girdi hangi değere düşer" kısmını kapsar.
/// page/pageSize ham string alınır (bkz. CourseCatalogQuery.Create remarks) — bu yüzden testler
/// hem sayısal olmayan hem de sayısal-ama-geçersiz (0/negatif) girdileri ayrı ayrı sınar.
/// </summary>
public class CourseCatalogQueryTests
{
    [Fact]
    public void Create_NoParams_DefaultsToPage1AndPageSize20()
    {
        var query = CourseCatalogQuery.Create(page: null, pageSize: null, categoryId: null);

        Assert.Equal(1, query.Page);
        Assert.Equal(20, query.PageSize);
    }

    [Fact]
    public void Create_PageAndPageSizeProvided_UsesProvidedValues()
    {
        var query = CourseCatalogQuery.Create(page: "3", pageSize: "10", categoryId: null);

        Assert.Equal(3, query.Page);
        Assert.Equal(10, query.PageSize);
    }

    [Fact]
    public void Create_PageSizeAboveMax_ClampsTo100()
    {
        var query = CourseCatalogQuery.Create(page: "1", pageSize: "500", categoryId: null);

        Assert.Equal(100, query.PageSize);
    }

    // QA bulgusu (düşük öncelik): pageSize=1 alt sınır değeri ayrıca pinlenmemişti.
    [Fact]
    public void Create_PageSizeExactlyOne_KeepsPageSizeOne()
    {
        var query = CourseCatalogQuery.Create(page: "1", pageSize: "1", categoryId: null);

        Assert.Equal(1, query.PageSize);
    }

    // QA bulgusu (yüksek öncelik): page biçimsel olarak geçerli ama aşırı büyük bir sayı olduğunda
    // (page-1)*pageSize hesabı int32'yi taşıp negatife sarılıyordu (repro: page="2147483647").
    // MaxPage clamp'i bu taşmayı, "aralık dışı sayfa" davranışını (AC-10) bozmadan engellemeli.
    [Fact]
    public void Create_PageBeyondMaxPage_ClampsToMaxPageWithoutOverflow()
    {
        var query = CourseCatalogQuery.Create(page: int.MaxValue.ToString(), pageSize: "100", categoryId: null);

        Assert.Equal(CourseCatalogQuery.MaxPage, query.Page);
        // (Page-1)*PageSize'ın int32 aritmetiğinde taşmadığını doğrudan doğrula — asıl hatanın nedeni buydu.
        var offset = (query.Page - 1) * query.PageSize;
        Assert.True(offset >= 0, $"Skip offset taştı: {offset}");
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("abc")]
    public void Create_InvalidPage_DefaultsToPage1(string invalidPage)
    {
        var query = CourseCatalogQuery.Create(page: invalidPage, pageSize: "20", categoryId: null);

        Assert.Equal(1, query.Page);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("xyz")]
    public void Create_InvalidPageSize_DefaultsToPageSize20(string invalidPageSize)
    {
        var query = CourseCatalogQuery.Create(page: "1", pageSize: invalidPageSize, categoryId: null);

        Assert.Equal(20, query.PageSize);
    }

    [Fact]
    public void Create_OnlyPageInvalid_KeepsProvidedValidPageSize()
    {
        var query = CourseCatalogQuery.Create(page: "-1", pageSize: "50", categoryId: null);

        Assert.Equal(1, query.Page);
        Assert.Equal(50, query.PageSize);
    }

    [Fact]
    public void Create_OnlyPageSizeInvalid_KeepsProvidedValidPage()
    {
        var query = CourseCatalogQuery.Create(page: "4", pageSize: "0", categoryId: null);

        Assert.Equal(4, query.Page);
        Assert.Equal(20, query.PageSize);
    }

    [Fact]
    public void Create_CategoryIdNull_HasNoFilterAndIsNotInvalid()
    {
        var query = CourseCatalogQuery.Create(page: null, pageSize: null, categoryId: null);

        Assert.Null(query.CategoryId);
        Assert.False(query.HasInvalidCategoryFilter);
    }

    [Fact]
    public void Create_CategoryIdValidGuid_ParsesCategoryId()
    {
        var categoryId = Guid.NewGuid();

        var query = CourseCatalogQuery.Create(page: null, pageSize: null, categoryId: categoryId.ToString());

        Assert.Equal(categoryId, query.CategoryId);
        Assert.False(query.HasInvalidCategoryFilter);
    }

    [Fact]
    public void Create_CategoryIdInvalidFormat_MarksFilterInvalid()
    {
        var query = CourseCatalogQuery.Create(page: null, pageSize: null, categoryId: "not-a-guid");

        Assert.Null(query.CategoryId);
        Assert.True(query.HasInvalidCategoryFilter);
    }
}
