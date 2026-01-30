using Ddap.Client.Core;
using Ddap.Client.Grpc;
using FluentAssertions;
using GrpcCore = Grpc.Core;

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

    [Fact]
    public async Task TestConnectionAsync_WithCancellation_ShouldReturnFalse()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel immediately

        // Act
        var result = await client.TestConnectionAsync(cts.Token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CreateClient_WithInvalidType_ShouldThrowDdapClientException()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act & Assert
        Action act = () => client.CreateClient<InvalidGrpcClient>();
        act.Should().Throw<DdapClientException>();
    }

    [Fact]
    public void IsConnected_AfterGetChannel_ShouldCheckChannelState()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act
        var channel = client.GetChannel(); // This creates the channel
        var isConnected = client.IsConnected;

        // Assert
        channel.Should().NotBeNull();
        // IsConnected checks State == Ready, which won't be true without actual connection
        // So it should be false here
        isConnected.Should().BeFalse();
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Action act = () => new DdapGrpcClient(null!);
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void GetChannel_CalledMultipleTimes_ShouldReturnSameInstance()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act
        var channel1 = client.GetChannel();
        var channel2 = client.GetChannel();

        // Assert
        channel1.Should().BeSameAs(channel2);
    }
}

// Test helper class without proper gRPC constructor
public class InvalidGrpcClient : GrpcCore.ClientBase<InvalidGrpcClient>
{
    // Intentionally no constructor that takes GrpcChannel
    public InvalidGrpcClient()
        : base() { }

    protected override InvalidGrpcClient NewInstance(
        GrpcCore.ClientBase.ClientBaseConfiguration configuration
    )
    {
        return new InvalidGrpcClient();
    }
}
