namespace FullStack.Domain.Common;

/// <summary>
/// Enumerates the types of errors that can occur in domain operations.
/// </summary>
public enum ResultErrorType
{
    /// <summary>No error.</summary>
    None,

    /// <summary>The requested resource was not found.</summary>
    NotFound,

    /// <summary>A conflict with existing state occurred.</summary>
    Conflict,

    /// <summary>One or more validation errors occurred.</summary>
    Validation,

    /// <summary>The operation is not authorized.</summary>
    Unauthorized,

    /// <summary>An internal error occurred.</summary>
    Internal
}
