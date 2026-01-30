using System.Net;
using Ddap.Client.Core;
using Ddap.Client.GraphQL;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace Ddap.Client.GraphQL.Tests;

public class DdapGraphQLClientTests
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
        var client = new DdapGraphQLClient(httpClient, options);

        // Act
        var result = await client.TestConnectionAsync();

        // Assert
        result.Should().BeTrue();
        client.IsConnected.Should().BeTrue();
    }

    [Fact]
    public void Constructor_ShouldSetBaseUrl()
    {
        // Arrange
        var httpClient = new HttpClient();
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };

        // Act
        var client = new DdapGraphQLClient(httpClient, options);

        // Assert
        client.BaseUrl.Should().Be("https://api.example.com");
    }

    [Fact]
    public void GraphQLRequest_ShouldBeCreatable()
    {
        // Arrange & Act
        var request = new GraphQLRequest
        {
            Query = "{ users { id name } }",
            Variables = new { id = 1 },
        };

        // Assert
        request.Query.Should().Be("{ users { id name } }");
        request.Variables.Should().NotBeNull();
    }
}
