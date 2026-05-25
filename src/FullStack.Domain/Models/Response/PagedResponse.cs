namespace FullStack.Domain.Models.Response;

/// <summary>
/// A paged response wrapping a collection of items with pagination metadata.
/// </summary>
/// <typeparam name="T">The type of items in the page.</typeparam>
public record PagedResponse<T> where T : class
{
    /// <summary>The items in the current page.</summary>
    public IReadOnlyList<T> Items { get; init; } = [];

    /// <summary>The total number of items across all pages.</summary>
    public int TotalCount { get; init; }

    /// <summary>The current page number.</summary>
    public int PageNumber { get; init; }

    /// <summary>The page size.</summary>
    public int PageSize { get; init; }

    /// <summary>Whether there is a next page.</summary>
    public bool HasNextPage => PageNumber * PageSize < TotalCount;

    /// <summary>Whether there is a previous page.</summary>
    public bool HasPreviousPage => PageNumber > 1;
}
