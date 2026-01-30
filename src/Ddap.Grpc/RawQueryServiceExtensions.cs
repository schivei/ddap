using Ddap.Auth.Policies;
using Ddap.Core;
using Ddap.Grpc.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Grpc;

/// <summary>
/// Extension methods for configuring raw query services in the DI container.
/// </summary>
public static class RawQueryServiceExtensions
{
    /// <summary>
    /// Adds raw query execution services to the DI container with the default (read-only) policy.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddRawQueryServices();
    /// </code>
    /// </example>
    public static IServiceCollection AddRawQueryServices(this IServiceCollection services)
    {
        return AddRawQueryServices(services, new DefaultRawQueryPolicy());
    }

    /// <summary>
    /// Adds raw query execution services to the DI container with a custom policy.
    /// Note: You must also register an IRawQueryExecutor implementation before calling this.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="policy">The raw query policy to use.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddSingleton&lt;IRawQueryExecutor, DapperRawQueryExecutor&gt;();
    /// services.AddRawQueryServices(new AllowAllRawQueryPolicy());
    /// </code>
    /// </example>
    public static IServiceCollection AddRawQueryServices(
        this IServiceCollection services,
        IRawQueryPolicy policy
    )
    {
        services.AddSingleton(policy);
        services.AddSingleton<RawQueryServiceImpl>();
        services.AddGrpc();
        return services;
    }

    /// <summary>
    /// Adds raw query execution services to the DI container with a custom policy factory.
    /// Note: You must also register an IRawQueryExecutor implementation before calling this.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="policyFactory">Factory function to create the policy.</param>
    /// <returns>The service collection for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddSingleton&lt;IRawQueryExecutor, DapperRawQueryExecutor&gt;();
    /// services.AddRawQueryServices(sp =>
    ///     new CustomRawQueryPolicy(sp.GetRequiredService&lt;IConfiguration&gt;())
    /// );
    /// </code>
    /// </example>
    public static IServiceCollection AddRawQueryServices(
        this IServiceCollection services,
        Func<IServiceProvider, IRawQueryPolicy> policyFactory
    )
    {
        services.AddSingleton(policyFactory);
        services.AddSingleton<RawQueryServiceImpl>();
        services.AddGrpc();
        return services;
    }
}
