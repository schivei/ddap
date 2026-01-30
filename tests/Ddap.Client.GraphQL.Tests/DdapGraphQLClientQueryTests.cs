using System.Net;
using System.Text.Json;
using Ddap.Client.Core;
using Ddap.Client.GraphQL;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace Ddap.Client.GraphQL.Tests;

public class DdapGraphQLClientQueryTests
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
    public async Task QueryAsync_WithSuccessResponse_ReturnsData()
    {
        // Arrange
        var response = new GraphQLResponse<UserData>
        {
            Data = new UserData
            {
                Users = new[]
                {
                    new User { Id = 1, Name = "John" },
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
        result.Data.Should().NotBeNull();
        result.Data!.Users.Should().HaveCount(1);
        result.Data.Users[0].Name.Should().Be("John");
    }

    [Fact]
    public async Task QueryAsync_WithVariables_ExecutesQuery()
    {
        // Arrange
        var response = new GraphQLResponse<UserData>
        {
            Data = new UserData
            {
                Users = new[]
                {
                    new User { Id = 1, Name = "John" },
                },
            },
        };
        var json = JsonSerializer.Serialize(response);

        var mockHandler = CreateMockHandler(HttpStatusCode.OK, json);
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.QueryAsync<UserData>(
            "query($id: Int!) { users(id: $id) { id name } }",
            new { id = 1 }
        );

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task QueryAsync_WithErrorResponse_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.InternalServerError, "");
        var client = CreateClient(mockHandler);

        // Act & Assert
        await Assert.ThrowsAsync<DdapApiException>(() =>
            client.QueryAsync<UserData>("{ users { id name } }")
        );
    }

    [Fact]
    public async Task QueryAsync_WithGraphQLErrors_ReturnsErrors()
    {
        // Arrange
        var response = new GraphQLResponse<UserData>
        {
            Errors = new[]
            {
                new GraphQLError
                {
                    Message = "Query error",
                    Locations = new[]
                    {
                        new GraphQLLocation { Line = 1, Column = 1 },
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
        result.Errors[0].Message.Should().Be("Query error");
    }

    [Fact]
    public async Task MutationAsync_WithSuccess_ReturnsData()
    {
        // Arrange
        var response = new GraphQLResponse<UserData>
        {
            Data = new UserData
            {
                Users = new[]
                {
                    new User { Id = 1, Name = "New User" },
                },
            },
        };
        var json = JsonSerializer.Serialize(response);

        var mockHandler = CreateMockHandler(HttpStatusCode.OK, json);
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.MutationAsync<UserData>(
            "mutation { createUser(name: \"New User\") { id name } }"
        );

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public void GraphQLError_ShouldHaveProperties()
    {
        // Arrange & Act
        var error = new GraphQLError
        {
            Message = "Test error",
            Locations = new[]
            {
                new GraphQLLocation { Line = 1, Column = 1 },
            },
            Path = new[] { "users", "0", "name" },
        };

        // Assert
        error.Message.Should().Be("Test error");
        error.Locations.Should().NotBeNull();
        error.Path.Should().NotBeNull();
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
            .ReturnsAsync(
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
