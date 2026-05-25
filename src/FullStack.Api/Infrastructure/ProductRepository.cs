using FullStack.Domain.Contracts;
using FullStack.Domain.Entities;
using FullStack.Domain.Models.Request;
using FullStack.Domain.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Infrastructure;

/// <summary>
/// EF Core implementation of the product repository.
/// </summary>
public class ProductRepository(AppDbContext context) : IProductRepository
{
    /// <inheritdoc/>
    public async Task<PagedResponse<Product>> GetPagedAsync(PagedRequest request, CancellationToken ct)
    {
        var query = context.Products.Include(p => p.Category).AsQueryable();

        query = request.SortBy.ToLowerInvariant() switch
        {
            "price" => request.Ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
            "createdat" => request.Ascending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
            _ => request.Ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name)
        };

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        return new PagedResponse<Product>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    /// <inheritdoc/>
    public async Task<Product?> GetByIdAsync(int id, CancellationToken ct)
    {
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct)
    {
        return await context.Products.AnyAsync(p => p.Name == name, ct);
    }

    /// <inheritdoc/>
    public async Task<Product> AddAsync(Product product, CancellationToken ct)
    {
        var entry = await context.Products.AddAsync(product, ct);
        return entry.Entity;
    }

    /// <inheritdoc/>
    public Task UpdateAsync(Product product, CancellationToken ct)
    {
        // Detach any existing tracked entity with the same key to avoid tracking conflicts
        var existing = context.Products.Local.FirstOrDefault(p => p.Id == product.Id);
        if (existing is not null)
        {
            context.Entry(existing).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
        }

        context.Products.Update(product);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var product = await context.Products.FindAsync(new object[] { id }, ct);
        if (product is not null)
        {
            context.Products.Remove(product);
        }
    }
}
