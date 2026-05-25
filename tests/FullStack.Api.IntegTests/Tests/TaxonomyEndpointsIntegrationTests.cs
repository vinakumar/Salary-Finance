using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using FullStack.Domain.Catalog.Classification.Hierarchy.Taxonomy;
using FullStack.Domain.Models;

namespace FullStack.Api.IntegTests.Tests;

[TestFixture]
public class TaxonomyEndpointsIntegrationTests
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
    public async Task GetTaxonomyNodes_ReturnsOk_WithNodes()
    {
        var response = await _client.GetAsync("/api/v1/taxonomy");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var nodes = await response.Content.ReadFromJsonAsync<List<ProductTaxonomyNode>>(JsonOptions);
        nodes.Should().NotBeNull();
        nodes!.Should().NotBeEmpty();
        nodes.Should().Contain(n => n.Label == "Electronics");
    }

    [Test]
    public async Task GetTaxonomyNode_ReturnsOk_WithPayload()
    {
        var response = await _client.GetAsync("/api/v1/taxonomy/5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await response.Content.ReadFromJsonAsync<GenericPayload<ProductTaxonomyNode>>(JsonOptions);
        payload.Should().NotBeNull();
        payload!.Data.Should().NotBeNull();
        payload.Data!.NodeId.Should().Be(5);
    }
}
