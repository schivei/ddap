using Ddap.Core;
using HotChocolate.Subscriptions;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Subscriptions;

/// <summary>
/// Provides extension methods for adding real-time subscription support to DDAP.
/// </summary>
public static class DdapSubscriptionsExtensions
{
    /// <summary>
    /// Adds real-time subscription support to the DDAP builder.
    /// Configures SignalR hubs and GraphQL subscriptions for entity change notifications.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options => {
    ///     options.ConnectionString = "...";
    /// })
    /// .AddGraphQL()
    /// .AddDdapSubscriptions();
    /// 
    /// // In application startup:
    /// app.MapHub&lt;EntityHub&gt;("/entityhub");
    /// 
    /// // Subscribe to entity changes in GraphQL:
    /// subscription {
    ///   onEntityChanged(entityName: "User") {
    ///     operation
    ///     entityName
    ///     entityId
    ///     data
    ///   }
    /// }
    /// </code>
    /// </example>
    public static IDdapBuilder AddDdapSubscriptions(this IDdapBuilder builder)
    {
        // Register SignalR with default options
        builder.Services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.MaximumReceiveMessageSize = 102400; // 100 KB
            options.StreamBufferCapacity = 10;
        });

        return builder;
    }
}
