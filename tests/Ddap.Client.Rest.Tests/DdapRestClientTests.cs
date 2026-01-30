using System.Net;
using Ddap.Client.Core;
using Ddap.Client.Rest;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace Ddap.Client.Rest.Tests;

public class DdapRestClientTests
{
    [Fact]
    public async Task TestConnectionAsync_WhenSuccessful_ReturnsTrue()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(
                new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(""),
                }
            );

        var httpClient = new HttpClient(mockHandler.Object);
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapRestClient(httpClient, options);

        // Act
        var result = await client.TestConnectionAsync();

        // Assert
        result.Should().BeTrue();
        client.IsConnected.Should().BeTrue();
    }

    [Fact]
    public async Task TestConnectionAsync_WhenFails_ReturnsFalse()
    {
        // Arrange
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException());

        var httpClient = new HttpClient(mockHandler.Object);
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapRestClient(httpClient, options);

        // Act
        var result = await client.TestConnectionAsync();

        // Assert
        result.Should().BeFalse();
        client.IsConnected.Should().BeFalse();
    }

    [Fact]
    public void Constructor_ShouldSetBaseUrl()
    {
        // Arrange
        var httpClient = new HttpClient();
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };

        // Act
        var client = new DdapRestClient(httpClient, options);

        // Assert
        client.BaseUrl.Should().Be("https://api.example.com");
    }
}
