using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace FullStack.ApiClient.Handlers;

/// <summary>
/// Delegating handler that logs HTTP request method, URL, duration, and response status.
/// </summary>
public class LoggingHandler(ILogger<LoggingHandler> logger) : DelegatingHandler
{
    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        logger.LogDebug("HTTP {Method} {Url} starting...",
            request.Method, request.RequestUri);

        HttpResponseMessage response;
        try
        {
            response = await base.SendAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "HTTP {Method} {Url} failed after {Duration}ms",
                request.Method, request.RequestUri, stopwatch.ElapsedMilliseconds);
            throw;
        }

        stopwatch.Stop();

        var statusCode = (int)response.StatusCode;

        if (statusCode >= 500)
        {
            logger.LogWarning("HTTP {Method} {Url} → {StatusCode} in {Duration}ms (Server Error)",
                request.Method, request.RequestUri, statusCode, stopwatch.ElapsedMilliseconds);
        }
        else if (statusCode >= 400)
        {
            logger.LogWarning("HTTP {Method} {Url} → {StatusCode} in {Duration}ms (Client Error)",
                request.Method, request.RequestUri, statusCode, stopwatch.ElapsedMilliseconds);
        }
        else
        {
            logger.LogDebug("HTTP {Method} {Url} → {StatusCode} in {Duration}ms",
                request.Method, request.RequestUri, statusCode, stopwatch.ElapsedMilliseconds);
        }

        return response;
    }
}
