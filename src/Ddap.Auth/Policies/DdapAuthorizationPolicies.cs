namespace Ddap.Auth.Policies;

/// <summary>
/// Defines standard authorization policies for DDAP operations.
/// These policies control access to entity data based on JWT claims.
/// </summary>
/// <example>
/// <code>
/// // Use in controllers:
/// [Authorize(Policy = DdapAuthorizationPolicies.Read)]
/// public async Task&lt;IActionResult&gt; GetEntity(string id) { }
///
/// [Authorize(Policy = DdapAuthorizationPolicies.Write)]
/// public async Task&lt;IActionResult&gt; UpdateEntity(string id, object data) { }
///
/// [Authorize(Policy = DdapAuthorizationPolicies.Admin)]
/// public async Task&lt;IActionResult&gt; DeleteEntity(string id) { }
/// </code>
/// </example>
public static class DdapAuthorizationPolicies
{
    /// <summary>
    /// Policy for read operations. Requires authenticated user with 'read', 'write', or 'admin' permission claim.
    /// </summary>
    public const string Read = "DdapRead";

    /// <summary>
    /// Policy for write operations (create, update). Requires authenticated user with 'write' or 'admin' permission claim.
    /// </summary>
    public const string Write = "DdapWrite";

    /// <summary>
    /// Policy for administrative operations (delete, configuration). Requires authenticated user with 'admin' permission claim.
    /// </summary>
    public const string Admin = "DdapAdmin";
}
