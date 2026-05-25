using FullStack.Domain.Entities;
using FullStack.Domain.Models.Request;
using FullStack.Domain.Models.Response;

namespace FullStack.Domain.Contracts;

/// <summary>
/// Repository contract for Product aggregate operations.
/// </summary>
public interface IProductRepository
{
    /// <summary>Gets a paged list of products.</summary>
    /// <param name="request">The paging and sorting parameters.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paged response of products.</returns>
    Task<PagedResponse<Product>> GetPagedAsync(PagedRequest request, CancellationToken ct);

    /// <summary>Gets a product by its identifier.</summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The product, or null if not found.</returns>
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);

    /// <summary>Checks whether a product with the given name already exists.</summary>
    /// <param name="name">The product name to check.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>True if a product with the name exists.</returns>
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);

    /// <summary>Adds a new product.</summary>
    /// <param name="product">The product to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The added product with generated identifier.</returns>
    Task<Product> AddAsync(Product product, CancellationToken ct);

    /// <summary>Updates an existing product.</summary>
    /// <param name="product">The product with updated values.</param>
    /// <param name="ct">Cancellation token.</param>
    Task UpdateAsync(Product product, CancellationToken ct);

    /// <summary>Deletes a product by its identifier.</summary>
    /// <param name="id">The product identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    Task DeleteAsync(int id, CancellationToken ct);
}
