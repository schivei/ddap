using Ddap.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ddap.Auth;

/// <summary>
/// Provides extension methods for adding authentication and authorization support to DDAP.
/// </summary>
public static class DdapAuthExtensions
{
    /// <summary>
    /// Adds JWT bearer authentication to the DDAP builder.
    /// Configures JWT token validation with customizable issuer, audience, and signing key.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <param name="issuer">The JWT token issuer.</param>
    /// <param name="audience">The JWT token audience.</param>
    /// <param name="signingKey">The secret key used for JWT signing.</param>
    /// <param name="requireHttpsMetadata">Whether to require HTTPS for metadata. Default is true. Set to false for development.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// // Production:
    /// services.AddDdap(options => {
    ///     options.ConnectionString = "...";
    /// })
    /// .AddDdapAuthentication("https://myapp.com", "ddap-api", "your-256-bit-secret")
    /// .AddDdapAuthorization();
    /// 
    /// // Development:
    /// .AddDdapAuthentication("https://myapp.com", "ddap-api", "your-256-bit-secret", requireHttpsMetadata: false)
    /// 
    /// // In controllers:
    /// [DdapAuthorize(DdapAuthorizationPolicies.Read)]
    /// public async Task&lt;IActionResult&gt; GetData() { }
    /// </code>
    /// </example>
    public static IDdapBuilder AddDdapAuthentication(
        this IDdapBuilder builder,
        string issuer,
        string audience,
        string signingKey,
        bool requireHttpsMetadata = true)
    {
        var key = Encoding.UTF8.GetBytes(signingKey);

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = requireHttpsMetadata;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
            });

        return builder;
    }

    /// <summary>
    /// Adds policy-based authorization to the DDAP builder.
    /// Configures standard authorization policies for Read, Write, and Admin operations.
    /// </summary>
    /// <param name="builder">The DDAP builder.</param>
    /// <returns>The DDAP builder for chaining.</returns>
    /// <example>
    /// <code>
    /// services.AddDdap(options => {
    ///     options.ConnectionString = "...";
    /// })
    /// .AddDdapAuthentication("issuer", "audience", "key")
    /// .AddDdapAuthorization();
    /// 
    /// // Use policies in controllers:
    /// [DdapAuthorize(DdapAuthorizationPolicies.Admin)]
    /// public async Task&lt;IActionResult&gt; DeleteEntity() { }
    /// </code>
    /// </example>
    public static IDdapBuilder AddDdapAuthorization(this IDdapBuilder builder)
    {
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.DdapAuthorizationPolicies.Read, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permission", "read", "write", "admin"));

            options.AddPolicy(Policies.DdapAuthorizationPolicies.Write, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permission", "write", "admin"));

            options.AddPolicy(Policies.DdapAuthorizationPolicies.Admin, policy =>
                policy.RequireAuthenticatedUser()
                      .RequireClaim("permission", "admin"));
        });

        return builder;
    }
}
