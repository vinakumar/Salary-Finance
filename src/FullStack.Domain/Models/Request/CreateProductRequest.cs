namespace FullStack.Domain.Models.Request;

/// <summary>
/// Request model for creating a new product.
/// </summary>
public record CreateProductRequest
{
    /// <summary>The product name.</summary>
    public required string Name { get; init; }

    /// <summary>The product price.</summary>
    public required decimal Price { get; init; }

    /// <summary>The category identifier.</summary>
    public required int CategoryId { get; init; }

    /// <summary>The product description.</summary>
    public string Description { get; init; } = string.Empty;
}
