using FullStack.Domain.Contracts;

namespace FullStack.Api.Infrastructure;

/// <summary>
/// EF Core implementation of the Unit of Work pattern.
/// </summary>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    /// <inheritdoc/>
    public async Task<int> CommitAsync(CancellationToken ct = default)
    {
        return await context.SaveChangesAsync(ct);
    }
}
