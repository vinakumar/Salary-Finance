using System.Net;
using System.Text.Json;
using FullStack.Domain.Models.Response;

namespace FullStack.ApiClient;

/// <summary>
/// Strongly-typed exception for API client errors, containing ProblemDetails information.
/// </summary>
public class ApiClientException : Exception
{
    /// <summary>The HTTP status code returned by the API.</summary>
    public int StatusCode { get; }

    /// <summary>The raw response body text.</summary>
    public string Response { get; } = string.Empty;

    /// <summary>The response headers.</summary>
    public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; } = new Dictionary<string, IEnumerable<string>>();

    /// <summary>The trace identifier from the API response.</summary>
    public string? TraceId { get; }

    /// <summary>The instance path from ProblemDetails.</summary>
    public string? Instance { get; }

    /// <summary>The field-level validation errors, if any.</summary>
    public IReadOnlyList<ApiErrorDetail> Errors { get; } = Array.Empty<ApiErrorDetail>();

    /// <summary>Initializes a new instance.</summary>
    public ApiClientException() { }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The error message.</param>
    public ApiClientException(string message) : base(message) { }

    /// <summary>Initializes a new instance with a message and inner exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiClientException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="response">The raw response body.</param>
    /// <param name="headers">The response headers.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiClientException(
        string message,
        int statusCode,
        string? response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers,
        Exception? innerException)
        : base(message + "\n\nStatus: " + statusCode + "\nResponse: \n" + (response ?? "(null)"), innerException)
    {
        StatusCode = statusCode;
        Response = response ?? string.Empty;
        Headers = headers;

        (TraceId, Instance, Errors) = ParseProblemDetails(response);
    }

    private static (string? TraceId, string? Instance, IReadOnlyList<ApiErrorDetail> Errors) ParseProblemDetails(string? content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return (null, null, Array.Empty<ApiErrorDetail>());
        }

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            var traceId = root.TryGetProperty("traceId", out var t) ? t.GetString() : null;
            var instance = root.TryGetProperty("instance", out var i) ? i.GetString() : null;
            var errors = new List<ApiErrorDetail>();

            if (root.TryGetProperty("errors", out var errorsProp) &&
                errorsProp.ValueKind == JsonValueKind.Array)
            {
                foreach (var error in errorsProp.EnumerateArray())
                {
                    var field = error.TryGetProperty("field", out var f) ? f.GetString() ?? "" : "";
                    var msg = error.TryGetProperty("message", out var m) ? m.GetString() ?? "" : "";
                    errors.Add(new ApiErrorDetail { Field = field, Message = msg });
                }
            }

            return (traceId, instance, errors);
        }
        catch (JsonException)
        {
            return (null, null, Array.Empty<ApiErrorDetail>());
        }
    }

    /// <summary>
    /// Creates an <see cref="ApiClientException"/> from an HTTP response message.
    /// </summary>
    /// <param name="response">The HTTP response.</param>
    /// <returns>A typed API client exception.</returns>
    public static async Task<ApiClientException> FromResponseAsync(HttpResponseMessage response)
    {
        var statusCode = (int)response.StatusCode;
        var content = await response.Content.ReadAsStringAsync();
        var headers = response.Headers.ToDictionary(
            h => h.Key,
            h => h.Value,
            StringComparer.OrdinalIgnoreCase);

        string message = $"API request failed with status {statusCode}";

        try
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            if (root.TryGetProperty("detail", out var detail))
            {
                message = detail.GetString() ?? message;
            }
            else if (root.TryGetProperty("title", out var title))
            {
                message = title.GetString() ?? message;
            }
        }
        catch (JsonException)
        {
            // Use default message
        }

        return new ApiClientException(message, statusCode, content, headers, null);
    }
}

/// <summary>
/// Generic API client exception that includes the deserialized result object.
/// </summary>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public class ApiClientException<TResult> : ApiClientException
{
    /// <summary>The deserialized result object from the error response.</summary>
    public TResult? Result { get; }

    /// <summary>Initializes a new instance.</summary>
    public ApiClientException() { }

    /// <summary>Initializes a new instance with a message.</summary>
    /// <param name="message">The error message.</param>
    public ApiClientException(string message) : base(message) { }

    /// <summary>Initializes a new instance with a message and inner exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiClientException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiClientException{TResult}"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="statusCode">The HTTP status code.</param>
    /// <param name="response">The raw response body.</param>
    /// <param name="headers">The response headers.</param>
    /// <param name="result">The deserialized error response object.</param>
    /// <param name="innerException">The inner exception.</param>
    public ApiClientException(
        string message,
        int statusCode,
        string? response,
        IReadOnlyDictionary<string, IEnumerable<string>> headers,
        TResult result,
        Exception? innerException)
        : base(message, statusCode, response, headers, innerException)
    {
        Result = result;
    }
}
