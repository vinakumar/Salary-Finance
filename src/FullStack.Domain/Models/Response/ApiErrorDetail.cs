namespace FullStack.Domain.Models.Response;

/// <summary>
/// Represents a single field-level error detail for API error responses.
/// </summary>
public record ApiErrorDetail
{
    /// <summary>The field that caused the error.</summary>
    public string Field { get; init; } = "";

    /// <summary>The error message.</summary>
    public string Message { get; init; } = "";
}
