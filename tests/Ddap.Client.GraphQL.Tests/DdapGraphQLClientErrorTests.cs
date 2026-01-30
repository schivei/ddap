using System.Net;
using System.Text.Json;
using Ddap.Client.Core;
using Ddap.Client.GraphQL;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace Ddap.Client.GraphQL.Tests;

public class DdapGraphQLClientErrorTests
{
    private class UserData
    {
        public User[] Users { get; set; } = Array.Empty<User>();
    }

    private class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task QueryAsync_WithNullResponse_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, "null");
        var client = CreateClient(mockHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DdapApiException>(() =>
            client.QueryAsync<UserData>("{ users { id name } }")
        );
        exception.Message.Should().Contain("Failed to deserialize");
    }

    [Fact]
    public async Task QueryAsync_WithInvalidJson_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, "invalid json");
        var client = CreateClient(mockHandler);

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() =>
            client.QueryAsync<UserData>("{ users { id name } }")
        );
    }

    [Fact]
    public async Task QueryAsync_WithBadRequest_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.BadRequest, "");
        var client = CreateClient(mockHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DdapApiException>(() =>
            client.QueryAsync<UserData>("{ users { id name } }")
        );
        exception.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task MutationAsync_WithNullResponse_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, "null");
        var client = CreateClient(mockHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DdapApiException>(() =>
            client.MutationAsync<UserData>("mutation { createUser(name: \"Test\") { id name } }")
        );
        exception.Message.Should().Contain("Failed to deserialize");
    }

    [Fact]
    public async Task QueryAsync_WithGraphQLErrorsWithPath_ReturnsErrors()
    {
        // Arrange
        var response = new GraphQLResponse<UserData>
        {
            Errors = new[]
            {
                new GraphQLError
                {
                    Message = "Field error",
                    Path = new[] { "users", "0", "name" },
                    Locations = new[]
                    {
                        new GraphQLLocation { Line = 2, Column = 5 },
                    },
                },
            },
        };
        var json = JsonSerializer.Serialize(response);

        var mockHandler = CreateMockHandler(HttpStatusCode.OK, json);
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.QueryAsync<UserData>("{ users { id name } }");

        // Assert
        result.Should().NotBeNull();
        result.Errors.Should().NotBeNull();
        result.Errors!.Length.Should().Be(1);
        result.Errors[0].Path.Should().NotBeNull();
        result.Errors[0].Path.Should().Contain("users");
    }

    [Fact]
    public async Task TestConnectionAsync_WithFailure_ReturnsFalse()
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
            .ThrowsAsync(new HttpRequestException("Connection failed"));

        var httpClient = new HttpClient(mockHandler.Object);
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        var client = new DdapGraphQLClient(httpClient, options);

        // Act
        var result = await client.TestConnectionAsync();

        // Assert
        result.Should().BeFalse();
    }

    private Mock<HttpMessageHandler> CreateMockHandler(HttpStatusCode statusCode, string content)
    {
        var mockHandler = new Mock<HttpMessageHandler>();
        mockHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(() =>
                new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content),
                }
            );
        return mockHandler;
    }

    private DdapGraphQLClient CreateClient(Mock<HttpMessageHandler> mockHandler)
    {
        var httpClient = new HttpClient(mockHandler.Object);
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        return new DdapGraphQLClient(httpClient, options);
    }
}
