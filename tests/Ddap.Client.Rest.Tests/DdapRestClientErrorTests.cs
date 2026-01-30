using System.Net;
using System.Text.Json;
using Ddap.Client.Core;
using Ddap.Client.Rest;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace Ddap.Client.Rest.Tests;

public class DdapRestClientErrorTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task CreateAsync_WithNullResponse_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, "null");
        var client = CreateClient(mockHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DdapApiException>(() =>
            client.CreateAsync("/api/entities", new TestEntity { Name = "Test" })
        );
        exception.Message.Should().Contain("deserialize");
    }

    [Fact]
    public async Task UpdateAsync_WithNullResponse_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, "null");
        var client = CreateClient(mockHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DdapApiException>(() =>
            client.UpdateAsync("/api/entities", 1, new TestEntity { Name = "Test" })
        );
        exception.Message.Should().Contain("deserialize");
    }

    [Fact]
    public async Task GetAsync_WithEmptyResponse_ReturnsEmptyArray()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, "[]");
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.GetAsync<TestEntity>("/api/entities");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WithBadRequest_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.BadRequest, "");
        var client = CreateClient(mockHandler);

        // Act & Assert
        await Assert.ThrowsAsync<DdapApiException>(() => client.DeleteAsync("/api/entities", 1));
    }

    [Fact]
    public async Task CreateAsync_WithBadRequest_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.BadRequest, "");
        var client = CreateClient(mockHandler);

        // Act & Assert
        await Assert.ThrowsAsync<DdapApiException>(() =>
            client.CreateAsync("/api/entities", new TestEntity())
        );
    }

    [Fact]
    public async Task UpdateAsync_WithBadRequest_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.BadRequest, "");
        var client = CreateClient(mockHandler);

        // Act & Assert
        await Assert.ThrowsAsync<DdapApiException>(() =>
            client.UpdateAsync("/api/entities", 1, new TestEntity())
        );
    }

    [Fact]
    public async Task GetByIdAsync_WithInternalServerError_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.InternalServerError, "");
        var client = CreateClient(mockHandler);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DdapApiException>(() =>
            client.GetByIdAsync<TestEntity>("/api/entities", 1)
        );
        exception.StatusCode.Should().Be(500);
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

    private DdapRestClient CreateClient(Mock<HttpMessageHandler> mockHandler)
    {
        var httpClient = new HttpClient(mockHandler.Object);
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        return new DdapRestClient(httpClient, options);
    }
}
