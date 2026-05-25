namespace FullStack.Domain.Models.Request;

/// <summary>
/// Request model for paged queries with sorting support.
/// </summary>
public record PagedRequest
{
    /// <summary>The page number (1-based).</summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>The number of items per page.</summary>
    public int PageSize { get; init; } = 20;

    /// <summary>The field to sort by.</summary>
    public string SortBy { get; init; } = "name";

    /// <summary>Whether to sort in ascending order.</summary>
    public bool Ascending { get; init; } = true;
}
