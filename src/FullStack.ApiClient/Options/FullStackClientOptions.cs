namespace FullStack.ApiClient.Options;

/// <summary>
/// Configuration options for the FullStack API client.
/// </summary>
public class FullStackClientOptions
{
    /// <summary>The base URL of the API.</summary>
    public string BaseUrl { get; set; } = "https://localhost:5001";

    /// <summary>HTTP request timeout in seconds.</summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>Optional bearer token for authentication.</summary>
    public string? BearerToken { get; set; }

    /// <summary>Whether to enable request/response logging.</summary>
    public bool EnableLogging { get; set; } = true;

    /// <summary>Maximum number of retries on transient failures.</summary>
    public int MaxRetries { get; set; } = 3;
}
