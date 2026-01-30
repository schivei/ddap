using Ddap.Client.Core;
using Ddap.Client.Grpc;
using FluentAssertions;

namespace Ddap.Client.Grpc.Tests;

public class DdapGrpcClientAdvancedTests
{
    [Fact]
    public void GetChannel_CalledMultipleTimes_ReturnsSameChannel()
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

    [Fact]
    public async Task TestConnectionAsync_WithInvalidUrl_ReturnsFalse()
    {
        // Arrange
        var options = new DdapClientOptions
        {
            BaseUrl = "http://invalid-url-that-does-not-exist.com",
        };
        var client = new DdapGrpcClient(options);

        // Act
        var result = await client.TestConnectionAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsConnected_BeforeConnection_ReturnsFalse()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act & Assert
        client.IsConnected.Should().BeFalse();
    }
}
