using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace Ddap.Client.Core;

/// <summary>
/// Provides resilience policies for HTTP clients
/// </summary>
public static class ResiliencePolicyProvider
{
    /// <summary>
    /// Creates a retry policy with exponential backoff
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(DdapClientOptions options)
    {
        if (options.UseExponentialBackoff)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    options.RetryCount,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        }

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(options.RetryCount, _ => options.RetryDelay);
    }

    /// <summary>
    /// Creates a circuit breaker policy
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
    }

    /// <summary>
    /// Creates a timeout policy
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(TimeSpan timeout)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(timeout);
    }
}
