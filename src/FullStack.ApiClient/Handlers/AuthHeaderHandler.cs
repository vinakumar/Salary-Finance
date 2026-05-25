using System.Net.Http.Headers;
using FullStack.ApiClient.Options;
using Microsoft.Extensions.Options;

namespace FullStack.ApiClient.Handlers;

/// <summary>
/// Delegating handler that injects Authorization Bearer token and X-Correlation-Id headers.
/// </summary>
public class AuthHeaderHandler(IOptions<FullStackClientOptions> options) : DelegatingHandler
{
    /// <inheritdoc/>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var opts = options.Value;

        if (!string.IsNullOrWhiteSpace(opts.BearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", opts.BearerToken);
        }

        if (!request.Headers.Contains("X-Correlation-Id"))
        {
            request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
