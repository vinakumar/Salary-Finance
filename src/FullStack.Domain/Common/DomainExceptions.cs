using FullStack.Domain.Models.Response;

namespace FullStack.Domain.Common;

/// <summary>
/// Base class for domain-specific exceptions.
/// </summary>
public abstract class DomainException : Exception
{
    /// <summary>Initializes a new instance.</summary>
    protected DomainException() { }

    /// <summary>Initializes a new instance with the specified message.</summary>
    /// <param name="message">The exception message.</param>
    protected DomainException(string message) : base(message) { }

    /// <summary>Initializes a new instance with a message and inner exception.</summary>
    /// <param name="message">The exception message.</param>
    /// <param name="innerException">The inner exception.</param>
    protected DomainException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Thrown when a requested entity is not found.
/// </summary>
#pragma warning disable RCS1194 // Custom domain exception with required parameters
public sealed class NotFoundException : DomainException
{
    /// <summary>Initializes a new instance for a missing entity.</summary>
    /// <param name="entity">The entity type name.</param>
    /// <param name="id">The entity identifier.</param>
    public NotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found.")
    {
        Entity = entity;
        EntityId = id;
    }

    /// <summary>The entity type that was not found.</summary>
    public string Entity { get; }

    /// <summary>The identifier that was searched for.</summary>
    public object EntityId { get; }
}

/// <summary>
/// Thrown when an operation conflicts with existing state.
/// </summary>
public sealed class ConflictException : DomainException
{
    /// <summary>Initializes a new instance with the conflict description.</summary>
    /// <param name="message">The conflict description.</param>
    public ConflictException(string message) : base(message) { }
}

/// <summary>
/// Thrown when one or more validation errors occur.
/// </summary>
public sealed class ValidationException : DomainException
{
    /// <summary>Initializes a new instance with validation errors.</summary>
    /// <param name="errors">The collection of validation errors.</param>
    public ValidationException(IEnumerable<ApiErrorDetail> errors)
        : base("One or more validation errors occurred.")
    {
        Errors = errors.ToList();
    }

    /// <summary>The validation errors.</summary>
    public IReadOnlyList<ApiErrorDetail> Errors { get; }
}
#pragma warning restore RCS1194
