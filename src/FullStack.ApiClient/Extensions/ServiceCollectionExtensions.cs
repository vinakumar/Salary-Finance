using FullStack.ApiClient.Handlers;
using FullStack.ApiClient.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FullStack.ApiClient.Extensions;

/// <summary>
/// Extension methods for registering the FullStack API client in the DI container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the FullStack API client with all handlers and typed HttpClient.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action for client options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddFullStackApiClient(
        this IServiceCollection services,
        Action<FullStackClientOptions>? configure = null)
    {
        if (configure is not null)
        {
            services.Configure(configure);
        }
        else
        {
            services.Configure<FullStackClientOptions>(_ => { });
        }

        services.AddTransient<AuthHeaderHandler>();
        services.AddTransient<LoggingHandler>();
        services.AddTransient<RetryHandler>();

        services.AddHttpClient("FullStackApi", (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<FullStackClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        })
        .AddHttpMessageHandler<AuthHeaderHandler>()
        .AddHttpMessageHandler<LoggingHandler>()
        .AddHttpMessageHandler<RetryHandler>();

        return services;
    }
}
