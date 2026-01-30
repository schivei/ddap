using Ddap.Client.Core;
using Ddap.Client.Grpc;
using FluentAssertions;

namespace Ddap.Client.Grpc.Tests;

public class DdapGrpcClientTests
{
    [Fact]
    public void Constructor_ShouldSetBaseUrl()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };

        // Act
        var client = new DdapGrpcClient(options);

        // Assert
        client.BaseUrl.Should().Be("https://api.example.com");
    }

    [Fact]
    public void GetChannel_ShouldCreateChannel()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act
        var channel = client.GetChannel();

        // Assert
        channel.Should().NotBeNull();
        channel.Target.Should().Contain("api.example.com");
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act & Assert - no exception should be thrown
        client.Dispose();
        client.Dispose();
    }

    [Fact]
    public void IsConnected_WhenChannelNotCreated_ShouldReturnFalse()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act & Assert - should return false when channel hasn't been created yet
        client.IsConnected.Should().BeFalse();
    }

    [Fact]
    public void Dispose_WhenChannelNotCreated_ShouldNotThrow()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act & Assert - should not throw when channel was never created
        Action act = () => client.Dispose();
        act.Should().NotThrow();
    }
}
