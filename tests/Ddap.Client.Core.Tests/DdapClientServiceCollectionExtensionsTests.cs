using Ddap.Client.Core;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Ddap.Client.Core.Tests;

public class DdapClientServiceCollectionExtensionsTests
{
    [Fact]
    public void AddDdapClientCore_ShouldRegisterServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapClientCore(options =>
        {
            options.BaseUrl = "https://api.example.com";
            options.Timeout = TimeSpan.FromSeconds(60);
        });

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<DdapClientOptions>();
        options.Should().NotBeNull();
        options!.BaseUrl.Should().Be("https://api.example.com");
        options.Timeout.Should().Be(TimeSpan.FromSeconds(60));
    }

    [Fact]
    public void AddDdapClientCore_WithoutOptions_ShouldRegisterDefaultOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddDdapClientCore();

        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var options = serviceProvider.GetService<DdapClientOptions>();
        options.Should().NotBeNull();
    }
}
