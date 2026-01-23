using Microsoft.AspNetCore.SignalR;

namespace Ddap.Subscriptions.Hubs;

/// <summary>
/// SignalR hub for real-time entity update notifications.
/// Clients can subscribe to entity changes and receive push notifications.
/// </summary>
/// <example>
/// <code>
/// // Client-side JavaScript example:
/// const connection = new signalR.HubConnectionBuilder()
///     .withUrl("/entityhub")
///     .build();
/// 
/// connection.on("EntityCreated", (entityName, entityId, data) => {
///     console.log(`Entity ${entityName} created:`, entityId, data);
/// });
/// 
/// connection.on("EntityUpdated", (entityName, entityId, data) => {
///     console.log(`Entity ${entityName} updated:`, entityId, data);
/// });
/// 
/// connection.on("EntityDeleted", (entityName, entityId) => {
///     console.log(`Entity ${entityName} deleted:`, entityId);
/// });
/// 
/// await connection.start();
/// await connection.invoke("SubscribeToEntity", "User");
/// </code>
/// </example>
public class EntityHub : Hub
{
    private readonly Dictionary<string, HashSet<string>> _subscriptions = new();
    private readonly object _lock = new();

    /// <summary>
    /// Subscribes the current connection to changes for a specific entity type.
    /// </summary>
    /// <param name="entityName">The name of the entity to subscribe to.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SubscribeToEntity(string entityName)
    {
        lock (_lock)
        {
            if (!_subscriptions.ContainsKey(entityName))
            {
                _subscriptions[entityName] = new HashSet<string>();
            }
            _subscriptions[entityName].Add(Context.ConnectionId);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, $"entity_{entityName}");
    }

    /// <summary>
    /// Unsubscribes the current connection from changes for a specific entity type.
    /// </summary>
    /// <param name="entityName">The name of the entity to unsubscribe from.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UnsubscribeFromEntity(string entityName)
    {
        lock (_lock)
        {
            if (_subscriptions.ContainsKey(entityName))
            {
                _subscriptions[entityName].Remove(Context.ConnectionId);
            }
        }

        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"entity_{entityName}");
    }

    /// <summary>
    /// Notifies subscribers that an entity was created.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <param name="entityId">The ID of the created entity.</param>
    /// <param name="data">The entity data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task NotifyEntityCreated(string entityName, string entityId, object data)
    {
        await Clients.Group($"entity_{entityName}").SendAsync("EntityCreated", entityName, entityId, data);
    }

    /// <summary>
    /// Notifies subscribers that an entity was updated.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <param name="entityId">The ID of the updated entity.</param>
    /// <param name="data">The updated entity data.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task NotifyEntityUpdated(string entityName, string entityId, object data)
    {
        await Clients.Group($"entity_{entityName}").SendAsync("EntityUpdated", entityName, entityId, data);
    }

    /// <summary>
    /// Notifies subscribers that an entity was deleted.
    /// </summary>
    /// <param name="entityName">The name of the entity.</param>
    /// <param name="entityId">The ID of the deleted entity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task NotifyEntityDeleted(string entityName, string entityId)
    {
        await Clients.Group($"entity_{entityName}").SendAsync("EntityDeleted", entityName, entityId);
    }

    /// <inheritdoc/>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        lock (_lock)
        {
            foreach (var subscription in _subscriptions.Values)
            {
                subscription.Remove(Context.ConnectionId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
