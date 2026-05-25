namespace FullStack.Domain.Common;

/// <summary>
/// A Result monad that encapsulates either a success value or an error.
/// Used to avoid exceptions for expected business logic failures.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public sealed class Result<T>
{
    /// <summary>Whether the operation succeeded.</summary>
    public bool IsSuccess { get; }

    /// <summary>The success value (only valid when IsSuccess is true).</summary>
    public T? Value { get; }

    /// <summary>The error message (only valid when IsSuccess is false).</summary>
    public string? Error { get; }

    /// <summary>The type of error that occurred.</summary>
    public ResultErrorType ErrorType { get; }

    private Result(bool isSuccess, T? value, string? error, ResultErrorType errorType)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ErrorType = errorType;
    }

    /// <summary>Creates a successful result with the given value.</summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful Result.</returns>
    public static Result<T> Success(T value) => new(true, value, null, ResultErrorType.None);

    /// <summary>Creates a failed result with the given error details.</summary>
    /// <param name="error">The error message.</param>
    /// <param name="type">The type of error.</param>
    /// <returns>A failed Result.</returns>
    public static Result<T> Failure(string error, ResultErrorType type) => new(false, default, error, type);
}
