namespace FullStack.Domain.Models;

/// <summary>
/// A generic payload wrapper with explicit non-default values for testing serialization behavior.
/// </summary>
/// <typeparam name="T">The type of the data payload.</typeparam>
public sealed class GenericPayload<T> where T : class
{
    /// <summary>The payload version.</summary>
    public int Version { get; set; } = 2;

    /// <summary>Whether the payload is active.</summary>
    public bool IsActive { get; set; } = true;

    /// <summary>The source system identifier.</summary>
    public string Source { get; set; } = "system";

    /// <summary>The correlation identifier for distributed tracing.</summary>
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>The data payload.</summary>
    public T? Data { get; set; }

    /// <summary>When the payload was created.</summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
