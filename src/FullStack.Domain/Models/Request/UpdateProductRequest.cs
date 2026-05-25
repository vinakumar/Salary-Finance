namespace FullStack.Domain.Models.Request;

/// <summary>
/// Request model for updating an existing product.
/// </summary>
public record UpdateProductRequest
{
    /// <summary>The updated product name, or null to leave unchanged.</summary>
    public string? Name { get; init; }

    /// <summary>The updated product price, or null to leave unchanged.</summary>
    public decimal? Price { get; init; }

    /// <summary>The updated product description, or null to leave unchanged.</summary>
    public string? Description { get; init; }
}
