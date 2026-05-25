using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FullStack.Domain.Models.Request;
using FullStack.Domain.Models.Response;

namespace FullStack.Api.IntegTests.Tests;

[TestFixture]
public class ProductsControllerIntegrationTests
{
    private CustomWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetProducts_ReturnsOk_WithPagedProducts()
    {
        var response = await _client.GetAsync("/api/v1/products");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paged = await response.Content.ReadFromJsonAsync<PagedResponse<ProductResponse>>(JsonOptions);
        paged.Should().NotBeNull();
        paged!.Items.Should().NotBeEmpty();
        paged.TotalCount.Should().BeGreaterThan(0);
    }

    [Test]
    public async Task GetProducts_SupportsPagination()
    {
        var response = await _client.GetAsync("/api/v1/products?pageNumber=1&pageSize=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var paged = await response.Content.ReadFromJsonAsync<PagedResponse<ProductResponse>>(JsonOptions);
        paged!.Items.Should().HaveCountLessOrEqualTo(5);
        paged.PageSize.Should().Be(5);
    }

    [Test]
    public async Task GetProductById_ReturnsOk_WhenProductExists()
    {
        var response = await _client.GetAsync("/api/v1/products/1");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductResponse>(JsonOptions);
        product.Should().NotBeNull();
        product!.Id.Should().Be(1);
        product.Name.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task GetProductById_Returns404_WhenProductDoesNotExist()
    {
        var response = await _client.GetAsync("/api/v1/products/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task CreateProduct_Returns201_WithValidData()
    {
        var request = new CreateProductRequest
        {
            Name = "Integration Test Product",
            Price = 42.99m,
            CategoryId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await response.Content.ReadFromJsonAsync<ProductResponse>(JsonOptions);
        created.Should().NotBeNull();
        created!.Name.Should().Be("Integration Test Product");
        created.Price.Should().Be(42.99m);
        response.Headers.Location.Should().NotBeNull();
    }

    [Test]
    public async Task CreateProduct_Returns422_WithInvalidData()
    {
        var request = new CreateProductRequest
        {
            Name = "",
            Price = 0m,
            CategoryId = 0
        };

        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async Task CreateProduct_Returns409_WhenNameAlreadyExists()
    {
        var request = new CreateProductRequest
        {
            Name = "Wireless Mouse", // seeded product name
            Price = 15m,
            CategoryId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Test]
    public async Task UpdateProduct_ReturnsOk_WithValidData()
    {
        var request = new UpdateProductRequest
        {
            Name = "Updated Product Name",
            Price = 99.99m
        };

        var response = await _client.PutAsJsonAsync("/api/v1/products/1", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<ProductResponse>(JsonOptions);
        updated!.Name.Should().Be("Updated Product Name");
        updated.Price.Should().Be(99.99m);
    }

    [Test]
    public async Task UpdateProduct_Returns404_WhenProductDoesNotExist()
    {
        var request = new UpdateProductRequest { Name = "Updated" };

        var response = await _client.PutAsJsonAsync("/api/v1/products/9999", request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteProduct_Returns204_WhenProductExists()
    {
        // First create a product to delete
        var createRequest = new CreateProductRequest
        {
            Name = "ToDelete Product",
            Price = 5m,
            CategoryId = 1
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/products", createRequest);
        var created = await createResponse.Content.ReadFromJsonAsync<ProductResponse>(JsonOptions);

        var response = await _client.DeleteAsync($"/api/v1/products/{created!.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeleteProduct_Returns404_WhenProductDoesNotExist()
    {
        var response = await _client.DeleteAsync("/api/v1/products/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
