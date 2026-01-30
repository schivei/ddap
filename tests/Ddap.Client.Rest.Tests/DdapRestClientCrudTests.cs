using System.Net;
using System.Text.Json;
using Ddap.Client.Core;
using Ddap.Client.Rest;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace Ddap.Client.Rest.Tests;

public class DdapRestClientCrudTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public async Task GetAsync_WithSuccessResponse_ReturnsEntities()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity { Id = 1, Name = "Test" },
        };
        var json = JsonSerializer.Serialize(entities);

        var mockHandler = CreateMockHandler(HttpStatusCode.OK, json);
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.GetAsync<TestEntity>("/api/entities");

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetAsync_WithErrorResponse_ThrowsException()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.InternalServerError, "");
        var client = CreateClient(mockHandler);

        // Act & Assert
        await Assert.ThrowsAsync<DdapApiException>(() =>
            client.GetAsync<TestEntity>("/api/entities")
        );
    }

    [Fact]
    public async Task GetByIdAsync_WithNotFound_ReturnsNull()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.NotFound, "");
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.GetByIdAsync<TestEntity>("/api/entities", 999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithSuccess_ReturnsEntity()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Test" };
        var json = JsonSerializer.Serialize(entity);
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, json);
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.GetByIdAsync<TestEntity>("/api/entities", 1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test");
    }

    [Fact]
    public async Task CreateAsync_WithSuccess_ReturnsCreatedEntity()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "New" };
        var json = JsonSerializer.Serialize(entity);
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, json);
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.CreateAsync("/api/entities", entity);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("New");
    }

    [Fact]
    public async Task UpdateAsync_WithSuccess_ReturnsUpdatedEntity()
    {
        // Arrange
        var entity = new TestEntity { Id = 1, Name = "Updated" };
        var json = JsonSerializer.Serialize(entity);
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, json);
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.UpdateAsync("/api/entities", 1, entity);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_WithSuccess_ReturnsTrue()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.OK, "");
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.DeleteAsync("/api/entities", 1);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WithNotFound_ReturnsFalse()
    {
        // Arrange
        var mockHandler = CreateMockHandler(HttpStatusCode.NotFound, "");
        var client = CreateClient(mockHandler);

        // Act
        var result = await client.DeleteAsync("/api/entities", 999);

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

    private DdapRestClient CreateClient(Mock<HttpMessageHandler> mockHandler)
    {
        var httpClient = new HttpClient(mockHandler.Object);
        var options = new DdapClientOptions { BaseUrl = "https://api.example.com" };
        return new DdapRestClient(httpClient, options);
    }
}
