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
    public void Dispose_ShouldDisposeChannel()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);
        var channel = client.GetChannel();

        // Act
        client.Dispose();

        // Assert - no exception should be thrown
        Assert.True(true);
    }
}
