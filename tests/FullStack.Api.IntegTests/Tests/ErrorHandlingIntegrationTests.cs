using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FullStack.Domain.Models.Request;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.IntegTests.Tests;

[TestFixture]
public class ErrorHandlingIntegrationTests
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
    public async Task NotFound_ReturnsProblemDetails_WithCorrectShape()
    {
        var response = await _client.GetAsync("/api/v1/products/9999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(404);
        problem.Title.Should().NotBeNullOrEmpty();
    }

    [Test]
    public async Task ValidationError_ReturnsProblemDetails_With422()
    {
        var request = new CreateProductRequest
        {
            Name = "",
            Price = -1m,
            CategoryId = 0
        };

        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("title");
        content.Should().Contain("status");
    }

    [Test]
    public async Task Conflict_ReturnsProblemDetails_With409()
    {
        var request = new CreateProductRequest
        {
            Name = "Wireless Mouse", // seeded product
            Price = 10m,
            CategoryId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/v1/products", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);
        problem.Should().NotBeNull();
        problem!.Status.Should().Be(409);
        problem.Detail.Should().Contain("already exists");
    }
}
