using System.Net;
using FullStack.ApiClient.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;

namespace FullStack.ApiClient.Handlers;

/// <summary>
/// Delegating handler that retries requests on transient failures using Polly.
/// Retries only on 5xx responses and network errors with exponential backoff.
/// </summary>
public class RetryHandler : DelegatingHandler
{
    private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetryHandler"/> class.
    /// </summary>
    /// <param name="options">Client configuration options.</param>
    /// <param name="logger">Logger instance.</param>
    public RetryHandler(IOptions<FullStackClientOptions> options, ILogger<RetryHandler> logger)
    {
        var maxRetries = options.Value.MaxRetries;

        _retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode >= HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(
                maxRetries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, _) =>
                {
                    logger.LogWarning(
                        "Retry {RetryCount}/{MaxRetries} after {Delay}s. Status: {Status}",
                        retryCount, maxRetries, timespan.TotalSeconds,
                        outcome.Result?.StatusCode.ToString() ?? outcome.Exception?.Message);
                });
    }

    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(
            ct => base.SendAsync(request, ct),
            cancellationToken);
    }
}
