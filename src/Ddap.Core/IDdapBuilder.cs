using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Core;

/// <summary>
/// Provides a builder for configuring DDAP services and providers.
/// </summary>
public interface IDdapBuilder
{
    /// <summary>
    /// Gets the service collection being configured.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the DDAP configuration options.
    /// </summary>
    DdapOptions Options { get; }
}

/// <summary>
/// Default implementation of <see cref="IDdapBuilder"/>.
/// </summary>
internal class DdapBuilder : IDdapBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DdapBuilder"/> class.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="options">The DDAP options.</param>
    public DdapBuilder(IServiceCollection services, DdapOptions options)
    {
        Services = services;
        Options = options;
    }

    /// <inheritdoc/>
    public IServiceCollection Services { get; }

    /// <inheritdoc/>
    public DdapOptions Options { get; }
}
