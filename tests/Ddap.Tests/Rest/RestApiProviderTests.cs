using Ddap.Core;
using Ddap.Rest;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ddap.Tests.Rest;

public class RestApiProviderTests
{
    [Fact]
    public void RestApiProvider_Should_Be_Registered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddRest();

        // Act
        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IRestApiProvider>();

        // Assert
        provider.Should().NotBeNull();
        provider.Should().BeAssignableTo<IRestApiProvider>();
    }

    [Fact]
    public void RestApiProvider_Should_Have_IsEnabled_True()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddRest();
        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IRestApiProvider>();

        // Act
        var isEnabled = provider!.IsEnabled;

        // Assert
        isEnabled.Should().BeTrue();
    }

    [Fact]
    public void RestApiProvider_Should_Have_Default_BasePath()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddRest();
        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IRestApiProvider>();

        // Act
        var basePath = provider!.BasePath;

        // Assert
        basePath.Should().Be("/api");
    }

    [Fact]
    public void RestApiProvider_IsEnabled_Should_Always_Return_True()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddRest();
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IRestApiProvider>();

        // Act
        var isEnabled1 = provider!.IsEnabled;
        var isEnabled2 = provider.IsEnabled;

        // Assert
        isEnabled1.Should().BeTrue();
        isEnabled2.Should().BeTrue();
        isEnabled1.Should().Be(isEnabled2);
    }

    [Fact]
    public void RestApiProvider_BasePath_Should_Always_Return_Same_Value()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddRest();
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IRestApiProvider>();

        // Act
        var basePath1 = provider!.BasePath;
        var basePath2 = provider.BasePath;

        // Assert
        basePath1.Should().Be(basePath2);
        basePath1.Should().NotBeNullOrEmpty();
    }
}
