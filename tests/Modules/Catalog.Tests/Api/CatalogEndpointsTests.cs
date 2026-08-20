using System.Net.Http.Json;
using Academy.Catalog.Api;

namespace Academy.Catalog.Tests.Api;

/// <summary>
/// GET /api/courses'un HTTP query-string binding davranışı — ASP.NET Core'un opsiyonel int?
/// parametreler için "sayısal olmayan değer → null" davranışının, Domain'in beklediği varsayılan
/// değerlere gerçekten düştüğünü uçtan uca doğrular (AC-07, AC-08).
/// </summary>
public class CatalogEndpointsTests : IClassFixture<CatalogWebApplicationFactory>
{
    private readonly CatalogWebApplicationFactory _factory;

    public CatalogEndpointsTests(CatalogWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_NoQueryString_ReturnsDefaultPageAndPageSize()
    {
        await _factory.InitializeDatabaseAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/courses");
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<CourseListResponse>();

        Assert.Equal(1, body!.Page);
        Assert.Equal(20, body.PageSize);
    }

    [Theory]
    [InlineData("page=abc")]
    [InlineData("page=0")]
    [InlineData("page=-1")]
    public async Task Get_InvalidPage_RequestNotRejected_DefaultsToPage1(string queryString)
    {
        await _factory.InitializeDatabaseAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/courses?{queryString}");

        response.EnsureSuccessStatusCode(); // istek reddedilmez (AC-07)
        var body = await response.Content.ReadFromJsonAsync<CourseListResponse>();
        Assert.Equal(1, body!.Page);
    }

    [Theory]
    [InlineData("pageSize=xyz")]
    [InlineData("pageSize=0")]
    [InlineData("pageSize=-1")]
    public async Task Get_InvalidPageSize_RequestNotRejected_DefaultsToPageSize20(string queryString)
    {
        await _factory.InitializeDatabaseAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/courses?{queryString}");

        response.EnsureSuccessStatusCode(); // istek reddedilmez (AC-08)
        var body = await response.Content.ReadFromJsonAsync<CourseListResponse>();
        Assert.Equal(20, body!.PageSize);
    }

    [Fact]
    public async Task Get_InvalidCategoryIdFormat_RequestNotRejected_ReturnsEmptyResult()
    {
        await _factory.InitializeDatabaseAsync();
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/courses?categoryId=not-a-guid");

        response.EnsureSuccessStatusCode(); // istek reddedilmez (AC-11)
        var body = await response.Content.ReadFromJsonAsync<CourseListResponse>();
        Assert.Empty(body!.Items);
        Assert.Equal(0, body.TotalCount);
    }
}
