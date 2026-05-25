namespace FullStack.Domain.Entities;

/// <summary>
/// Represents a product category in the catalog.
/// </summary>
/// <param name="Id">The unique identifier of the category.</param>
/// <param name="Name">The display name of the category.</param>
/// <param name="Slug">The URL-friendly slug for the category.</param>
public record ProductCategory(int Id, string Name, string Slug);
