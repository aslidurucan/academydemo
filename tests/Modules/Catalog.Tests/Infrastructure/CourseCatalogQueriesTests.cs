using Academy.Catalog.Domain;
using Academy.Catalog.Infrastructure;

namespace Academy.Catalog.Tests.Infrastructure;

/// <summary>
/// CourseCatalogQueries'in gerçek bir SQL motoruna (SQLite) karşı çalıştırdığı sorguları sınar.
/// Spec 0001'in tüm Acceptance Criteria'sı burada eşlenir (bkz. specs/0001-urun-listeleme.md).
/// </summary>
public class CourseCatalogQueriesTests : IDisposable
{
    private readonly CatalogSqliteFixture _fixture = new();
    private readonly CourseCatalogQueries _sut;

    public CourseCatalogQueriesTests()
    {
        _sut = new CourseCatalogQueries(_fixture.Context);
    }

    public void Dispose() => _fixture.Dispose();

    // AC-01: parametresiz istek → varsayılan sayfa (1) ve varsayılan sayfa boyutu (20).
    [Fact]
    public async Task GetPublishedCourses_NoParams_ReturnsFirstDefaultPage()
    {
        var categoryId = SeedCategory("Programlama");
        SeedPublishedCourses(categoryId, count: 3);

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, null));

        Assert.Equal(1, page.Page);
        Assert.Equal(20, page.PageSize);
        Assert.Equal(3, page.TotalCount);
        Assert.Equal(3, page.Items.Count);
    }

    // AC-02: page + pageSize birlikte verildiğinde belirtilen dilim döner.
    [Fact]
    public async Task GetPublishedCourses_ValidPageAndPageSize_ReturnsRequestedSlice()
    {
        var categoryId = SeedCategory("Programlama");
        var courses = SeedPublishedCourses(categoryId, count: 5); // en yeni ilk sırada (PublishedAt DESC)

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(page: "2", pageSize: "2", categoryId: null));

        Assert.Equal(2, page.Items.Count);
        Assert.Equal(courses[2].Id, page.Items[0].Id); // 3. ve 4. en yeni (skip 2, take 2)
        Assert.Equal(courses[3].Id, page.Items[1].Id);
    }

    // AC-03: pageSize üst sınır 100'den büyük istenirse sonuç 100 ile sınırlandırılır.
    [Fact]
    public async Task GetPublishedCourses_PageSizeAbove100_LimitsTo100Items()
    {
        var categoryId = SeedCategory("Programlama");
        SeedPublishedCourses(categoryId, count: 150);

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(page: "1", pageSize: "500", categoryId: null));

        Assert.Equal(100, page.PageSize);
        Assert.Equal(100, page.Items.Count);
        Assert.Equal(150, page.TotalCount);
    }

    // AC-04: katalogda hiç kurs yokken boş liste + totalCount:0 (hata değil).
    [Fact]
    public async Task GetPublishedCourses_EmptyCatalog_ReturnsEmptyResultWithZeroTotal()
    {
        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, null));

        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalCount);
    }

    // AC-05: categoryId ile filtre uygulandığında yalnızca o kategoriye ait kurslar döner.
    [Fact]
    public async Task GetPublishedCourses_WithCategoryFilter_ReturnsOnlyMatchingCategory()
    {
        var categoryA = SeedCategory("Programlama");
        var categoryB = SeedCategory("Tasarım");
        SeedPublishedCourses(categoryA, count: 2);
        SeedPublishedCourses(categoryB, count: 3);

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, categoryA.ToString()));

        Assert.Equal(2, page.TotalCount);
        Assert.All(page.Items, item => Assert.Equal("Programlama", item.CategoryName));
    }

    // AC-06: var olmayan/boş kategori → boş liste + totalCount:0 (hata değil).
    [Fact]
    public async Task GetPublishedCourses_CategoryWithNoCourses_ReturnsEmptyResult()
    {
        var categoryWithCourses = SeedCategory("Programlama");
        SeedPublishedCourses(categoryWithCourses, count: 2);
        var emptyCategoryId = SeedCategory("Boş Kategori");

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, emptyCategoryId.ToString()));

        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalCount);
    }

    // AC-10: toplam sonuçtan büyük bir sayfa numarası → hata dönmez, boş liste + doğru totalCount.
    [Fact]
    public async Task GetPublishedCourses_PageBeyondRange_ReturnsEmptyItemsWithCorrectTotalCount()
    {
        var categoryId = SeedCategory("Programlama");
        SeedPublishedCourses(categoryId, count: 3);

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(page: "10", pageSize: "20", categoryId: null));

        Assert.Empty(page.Items);
        Assert.Equal(3, page.TotalCount);
    }

    // AC-11: geçersiz formatlı categoryId → boş liste + totalCount:0 (hata değil).
    [Fact]
    public async Task GetPublishedCourses_InvalidCategoryIdFormat_ReturnsEmptyResult()
    {
        var categoryId = SeedCategory("Programlama");
        SeedPublishedCourses(categoryId, count: 3);

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, "not-a-guid"));

        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalCount);
    }

    // Regresyon ayrımı: categoryId hiç verilmemesi ("filtre yok"), geçersiz formatla ("her zaman boş")
    // karıştırılmamalı — biri tüm kursları, diğeri boş sonucu döndürmeli.
    [Fact]
    public async Task GetPublishedCourses_NoCategoryFilter_ReturnsAllPublishedCourses()
    {
        var categoryA = SeedCategory("Programlama");
        var categoryB = SeedCategory("Tasarım");
        SeedPublishedCourses(categoryA, count: 2);
        SeedPublishedCourses(categoryB, count: 3);

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, categoryId: null));

        Assert.Equal(5, page.TotalCount);
    }

    // AC-12: yayınlanmamış (taslak) hiçbir kurs, hiçbir sayfada/kategori filtresinde görünmez.
    [Fact]
    public async Task GetPublishedCourses_DraftCourses_NeverAppearInResults()
    {
        var categoryId = SeedCategory("Programlama");
        SeedPublishedCourses(categoryId, count: 2);
        SeedDraftCourse(categoryId);

        var unfiltered = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, null));
        var filtered = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, categoryId.ToString()));

        Assert.Equal(2, unfiltered.TotalCount);
        Assert.Equal(2, filtered.TotalCount);
        Assert.DoesNotContain(unfiltered.Items, i => i.Title == DraftTitle);
        Assert.DoesNotContain(filtered.Items, i => i.Title == DraftTitle);
    }

    // AC-13: aynı filtre/sayfalama parametreleriyle art arda yapılan istekler her zaman
    // yayın tarihine göre en yeniden eskiye aynı sırada döner — sayfalar arası kayma/tekrar olmaz.
    // PublishedAt değeri eşit olan kayıtlar da (tie) dahil edilir; Id ikincil sıralama tutarlılığı garantiler.
    [Fact]
    public async Task GetPublishedCourses_ConsecutiveRequests_ReturnConsistentOrderAcrossPages()
    {
        var categoryId = SeedCategory("Programlama");
        var tiePublishedAt = DateTime.UtcNow;
        SeedPublishedCourse(categoryId, tiePublishedAt); // aynı PublishedAt (tie) — iki kayıt
        SeedPublishedCourse(categoryId, tiePublishedAt);
        SeedPublishedCourses(categoryId, count: 3);

        var expected = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(page: "1", pageSize: "100", categoryId: null));

        var page1 = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(page: "1", pageSize: "2", categoryId: null));
        var page2 = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(page: "2", pageSize: "2", categoryId: null));
        var page3 = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(page: "3", pageSize: "2", categoryId: null));

        var actualIds = page1.Items.Concat(page2.Items).Concat(page3.Items).Select(i => i.Id).ToList();
        var expectedIds = expected.Items.Select(i => i.Id).ToList();

        Assert.Equal(expectedIds, actualIds); // sıra birebir aynı, tekrar/kayıp yok
        Assert.Equal(expectedIds.Distinct().Count(), expectedIds.Count); // tekrar yok
    }

    // AC-14: liste öğesi zorunlu asgari alanları içerir (id, başlık, fiyat, kategori adı, eğitmen adı, kapak url).
    [Fact]
    public async Task GetPublishedCourses_ReturnsRequiredFieldsPerItem()
    {
        var categoryId = SeedCategory("Programlama");
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = "ASP.NET Core ile API Geliştirme",
            ListPrice = 249.90m,
            CategoryId = categoryId,
            Status = PublicationStatus.Published,
            PublishedAt = DateTime.UtcNow,
            CoverImageUrl = "https://cdn.academy.test/covers/aspnet.jpg",
            InstructorId = Guid.NewGuid(),
            InstructorName = "Ayşe Yılmaz"
        };
        _fixture.Context.Courses.Add(course);
        await _fixture.Context.SaveChangesAsync();

        var page = await _sut.GetPublishedCoursesAsync(CourseCatalogQuery.Create(null, null, null));

        var item = Assert.Single(page.Items);
        Assert.Equal(course.Id, item.Id);
        Assert.Equal(course.Title, item.Title);
        Assert.Equal(course.ListPrice, item.ListPrice);
        Assert.Equal("Programlama", item.CategoryName);
        Assert.Equal(course.InstructorName, item.InstructorName);
        Assert.Equal(course.CoverImageUrl, item.CoverImageUrl);
    }

    private const string DraftTitle = "Yayınlanmamış Taslak Kurs";

    private Guid SeedCategory(string name)
    {
        var category = new Category { Id = Guid.NewGuid(), Name = name };
        _fixture.Context.Categories.Add(category);
        _fixture.Context.SaveChanges();
        return category.Id;
    }

    /// <summary>count adet yayınlanmış kurs, en eskisi ilk elemanda olacak şekilde üretir ve
    /// PublishedAt DESC sıralamasındaki (en yeniden en eskiye) hâliyle döner.</summary>
    private List<Course> SeedPublishedCourses(Guid categoryId, int count)
    {
        var baseTime = DateTime.UtcNow.AddDays(-count);
        var courses = new List<Course>();
        for (var i = 0; i < count; i++)
        {
            courses.Add(SeedPublishedCourse(categoryId, baseTime.AddDays(i)));
        }

        return courses.OrderByDescending(c => c.PublishedAt).ToList();
    }

    private Course SeedPublishedCourse(Guid categoryId, DateTime publishedAt)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = $"Kurs {Guid.NewGuid():N}",
            ListPrice = 99.90m,
            CategoryId = categoryId,
            Status = PublicationStatus.Published,
            PublishedAt = publishedAt,
            CoverImageUrl = "https://cdn.academy.test/covers/default.jpg",
            InstructorId = Guid.NewGuid(),
            InstructorName = "Eğitmen"
        };
        _fixture.Context.Courses.Add(course);
        _fixture.Context.SaveChanges();
        return course;
    }

    private void SeedDraftCourse(Guid categoryId)
    {
        var course = new Course
        {
            Id = Guid.NewGuid(),
            Title = DraftTitle,
            ListPrice = 49.90m,
            CategoryId = categoryId,
            Status = PublicationStatus.Draft,
            PublishedAt = null,
            CoverImageUrl = "https://cdn.academy.test/covers/draft.jpg",
            InstructorId = Guid.NewGuid(),
            InstructorName = "Eğitmen"
        };
        _fixture.Context.Courses.Add(course);
        _fixture.Context.SaveChanges();
    }
}
