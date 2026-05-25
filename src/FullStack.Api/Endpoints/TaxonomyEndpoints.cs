using FullStack.Domain.Catalog.Classification.Hierarchy.Taxonomy;
using FullStack.Domain.Models;

namespace FullStack.Api.Endpoints;

/// <summary>
/// Minimal API endpoints for product taxonomy operations.
/// </summary>
public static class TaxonomyEndpoints
{
    /// <summary>
    /// Maps taxonomy endpoints to the application.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The route group builder.</returns>
    public static RouteGroupBuilder MapTaxonomyEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("api/v1/taxonomy")
            .WithTags("Taxonomy");

        group.MapGet("/", GetTaxonomyNodes)
            .WithName("GetTaxonomyNodes")
            .WithSummary("Gets all taxonomy nodes")
            .Produces<IReadOnlyList<ProductTaxonomyNode>>();

        group.MapGet("/{nodeId:int}", GetTaxonomyNode)
            .WithName("GetTaxonomyNode")
            .WithSummary("Gets a taxonomy node by its identifier wrapped in a generic payload")
            .Produces<GenericPayload<ProductTaxonomyNode>>();

        group.MapGet("/internal/sync", InternalSync)
            .WithName("InternalSync")
            .ExcludeFromDescription();

        return group!;
    }

    private static IResult GetTaxonomyNodes()
    {
        var nodes = new List<ProductTaxonomyNode>
        {
            new()
            {
                NodeId = 1,
                Label = "Electronics",
                Depth = 0,
                IsLeaf = false,
                Children =
                [
                    new() { NodeId = 11, Label = "Computers", Depth = 1, IsLeaf = true },
                    new() { NodeId = 12, Label = "Phones", Depth = 1, IsLeaf = true }
                ]
            },
            new()
            {
                NodeId = 2,
                Label = "Clothing",
                Depth = 0,
                IsLeaf = false,
                Children =
                [
                    new() { NodeId = 21, Label = "Men", Depth = 1, IsLeaf = true },
                    new() { NodeId = 22, Label = "Women", Depth = 1, IsLeaf = true }
                ]
            },
            new()
            {
                NodeId = 3,
                Label = "Books",
                Depth = 0,
                IsLeaf = true
            }
        };

        return Results.Ok(nodes as IReadOnlyList<ProductTaxonomyNode>);
    }

    private static IResult GetTaxonomyNode(int nodeId)
    {
        var node = new ProductTaxonomyNode
        {
            NodeId = nodeId,
            Label = $"Node-{nodeId}",
            Depth = 1,
            IsLeaf = true
        };

        var payload = new GenericPayload<ProductTaxonomyNode>
        {
            Data = node
        };

        return Results.Ok(payload);
    }

    private static IResult InternalSync()
    {
        return Results.Ok(new { Status = "synced", Timestamp = DateTimeOffset.UtcNow });
    }
}
