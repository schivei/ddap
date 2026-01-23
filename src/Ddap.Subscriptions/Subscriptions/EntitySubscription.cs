using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Subscriptions;

namespace Ddap.Subscriptions.Subscriptions;

/// <summary>
/// GraphQL subscription type for entity changes.
/// This class is partial to allow for extensibility through source generation.
/// </summary>
/// <example>
/// <code>
/// // GraphQL subscription query:
/// subscription {
///   onEntityChanged(entityName: "User") {
///     operation
///     entityName
///     entityId
///     timestamp
///     data
///   }
/// }
/// 
/// // To publish changes from code:
/// await eventSender.SendAsync("entity_changed_User", new EntityChangeNotification
/// {
///     Operation = "CREATE",
///     EntityName = "User",
///     EntityId = "123",
///     Timestamp = DateTime.UtcNow,
///     Data = userData
/// });
/// </code>
/// </example>
public partial class EntitySubscription
{
    /// <summary>
    /// Subscribes to entity change notifications for a specific entity type.
    /// </summary>
    /// <param name="entityName">The name of the entity to subscribe to.</param>
    /// <param name="eventReceiver">The GraphQL event receiver.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An async stream of entity change notifications.</returns>
    [Subscribe]
    [Topic("entity_changed_{entityName}")]
    public async IAsyncEnumerable<EntityChangeNotification> OnEntityChanged(
        string entityName,
        [Service] ITopicEventReceiver eventReceiver,
        CancellationToken cancellationToken = default)
    {
        var stream = await eventReceiver.SubscribeAsync<EntityChangeNotification>(
            $"entity_changed_{entityName}", 
            cancellationToken);

        await foreach (var notification in stream.ReadEventsAsync().WithCancellation(cancellationToken))
        {
            yield return notification;
        }
    }
}

/// <summary>
/// Represents a notification about an entity change.
/// </summary>
public class EntityChangeNotification
{
    /// <summary>
    /// Gets or sets the operation type (CREATE, UPDATE, DELETE).
    /// </summary>
    public string Operation { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the entity that changed.
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ID of the entity that changed.
    /// </summary>
    public string EntityId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timestamp of the change.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the entity data (for CREATE and UPDATE operations).
    /// </summary>
    public object? Data { get; set; }
}
