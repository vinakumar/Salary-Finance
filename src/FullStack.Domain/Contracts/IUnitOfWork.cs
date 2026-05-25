namespace FullStack.Domain.Contracts;

/// <summary>
/// Unit of Work contract for coordinating transactional persistence.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Commits all pending changes as a single transaction.</summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The number of state entries written.</returns>
    Task<int> CommitAsync(CancellationToken ct = default);
}
