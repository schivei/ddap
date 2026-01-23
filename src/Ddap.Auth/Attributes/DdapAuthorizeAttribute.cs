using Microsoft.AspNetCore.Authorization;

namespace Ddap.Auth.Attributes;

/// <summary>
/// Authorization attribute for decorating DDAP controllers with policy-based authorization.
/// Simplifies applying DDAP authorization policies to controller actions.
/// </summary>
/// <example>
/// <code>
/// [DdapAuthorize(DdapAuthorizationPolicies.Read)]
/// public async Task&lt;IActionResult&gt; GetEntity(string id)
/// {
///     // Only users with read permission can access
/// }
///
/// [DdapAuthorize(DdapAuthorizationPolicies.Write)]
/// public async Task&lt;IActionResult&gt; UpdateEntity(string id, EntityData data)
/// {
///     // Only users with write permission can access
/// }
///
/// [DdapAuthorize(DdapAuthorizationPolicies.Admin)]
/// public async Task&lt;IActionResult&gt; DeleteEntity(string id)
/// {
///     // Only users with admin permission can access
/// }
/// </code>
/// </example>
[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Method,
    AllowMultiple = true,
    Inherited = true
)]
public class DdapAuthorizeAttribute : AuthorizeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DdapAuthorizeAttribute"/> class.
    /// </summary>
    /// <param name="policy">The authorization policy name to apply.</param>
    public DdapAuthorizeAttribute(string policy)
    {
        Policy = policy;
    }
}
