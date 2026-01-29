using Ddap.Core;
using Ddap.Grpc;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Ddap.Tests.Grpc;

public class GrpcServiceProviderTests
{
    [Fact]
    public void GrpcServiceProvider_Should_Be_Registered()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddGrpc();

        // Act
        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IGrpcServiceProvider>();

        // Assert
        provider.Should().NotBeNull();
        provider.Should().BeAssignableTo<IGrpcServiceProvider>();
    }

    [Fact]
    public void GrpcServiceProvider_Should_Have_IsEnabled_True()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddGrpc();
        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IGrpcServiceProvider>();

        // Act
        var isEnabled = provider!.IsEnabled;

        // Assert
        isEnabled.Should().BeTrue();
    }

    [Fact]
    public void GrpcServiceProvider_Should_Have_Default_ProtoFilePath()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddGrpc();
        using var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IGrpcServiceProvider>();

        // Act
        var protoFilePath = provider!.ProtoFilePath;

        // Assert
        protoFilePath.Should().Be("/proto/ddap.proto");
    }

    [Fact]
    public void GrpcServiceProvider_IsEnabled_Should_Always_Return_True()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddGrpc();
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IGrpcServiceProvider>();

        // Act
        var isEnabled1 = provider!.IsEnabled;
        var isEnabled2 = provider.IsEnabled;

        // Assert
        isEnabled1.Should().BeTrue();
        isEnabled2.Should().BeTrue();
        isEnabled1.Should().Be(isEnabled2);
    }

    [Fact]
    public void GrpcServiceProvider_ProtoFilePath_Should_Always_Return_Same_Value()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddDdap(options => { }).AddGrpc();
        var serviceProvider = services.BuildServiceProvider();
        var provider = serviceProvider.GetService<IGrpcServiceProvider>();

        // Act
        var path1 = provider!.ProtoFilePath;
        var path2 = provider.ProtoFilePath;

        // Assert
        path1.Should().Be(path2);
        path1.Should().NotBeNullOrEmpty();
    }
}
