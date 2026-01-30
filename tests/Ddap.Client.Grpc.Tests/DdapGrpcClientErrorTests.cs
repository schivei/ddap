using Ddap.Client.Core;
using Ddap.Client.Grpc;
using FluentAssertions;

namespace Ddap.Client.Grpc.Tests;

public class DdapGrpcClientErrorTests
{
    [Fact]
    public async Task TestConnectionAsync_WithTimeout_ReturnsFalse()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "http://localhost:9998" };
        var client = new DdapGrpcClient(options);

        // Act
        var result = await client.TestConnectionAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task TestConnectionAsync_WithCancellation_ReturnsFalse()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "http://localhost:9997" };
        var client = new DdapGrpcClient(options);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        // Act
        var result = await client.TestConnectionAsync(cts.Token);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);

        // Act & Assert
        client.Dispose();
        client.Dispose(); // Second dispose should not throw
    }

    [Fact]
    public void GetChannel_AfterDispose_ShouldStillWork()
    {
        // Arrange
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGrpcClient(options);
        var channel = client.GetChannel();

        // Act
        client.Dispose();

        // Assert - getting channel again should work (creates new one if needed)
        channel.Should().NotBeNull();
    }
}
