using Aspire.Hosting.ApplicationModel;

namespace Ddap.Aspire;

/// <summary>
/// Represents a DDAP resource in an Aspire application.
/// This resource provides automatic API generation from database schemas
/// for rapid agile development.
/// </summary>
public class DdapResource : Resource, IResourceWithEndpoints
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DdapResource"/> class.
    /// </summary>
    /// <param name="name">The name of the resource.</param>
    public DdapResource(string name)
        : base(name) { }

    /// <summary>
    /// Gets or sets a value indicating whether REST API is enabled.
    /// </summary>
    public bool EnableRest { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether GraphQL is enabled.
    /// </summary>
    public bool EnableGraphQL { get; set; }

    /// <summary>
    /// Gets or sets the GraphQL endpoint path.
    /// </summary>
    public string GraphQLPath { get; set; } = "/graphql";

    /// <summary>
    /// Gets or sets a value indicating whether gRPC is enabled.
    /// </summary>
    public bool EnableGrpc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether automatic schema refresh is enabled.
    /// </summary>
    public bool AutoRefresh { get; set; }

    /// <summary>
    /// Gets or sets the refresh interval in seconds.
    /// </summary>
    public int RefreshIntervalSeconds { get; set; } = 30;
}
