namespace FullStack.Domain.Entities;

/// <summary>
/// Represents a product in the catalog.
/// </summary>
public record Product
{
    /// <summary>The unique identifier of the product.</summary>
    public int Id { get; init; }

    /// <summary>The product name.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>The product price.</summary>
    public decimal Price { get; init; }

    /// <summary>The product description.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>The category foreign key.</summary>
    public int CategoryId { get; init; }

    /// <summary>The category navigation property.</summary>
    public ProductCategory Category { get; init; } = null!;

    /// <summary>When the product was created.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Whether the product is currently active.</summary>
    public bool IsActive { get; init; }
}

