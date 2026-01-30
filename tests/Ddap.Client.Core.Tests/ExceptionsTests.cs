using Ddap.Client.Core;
using FluentAssertions;

namespace Ddap.Client.Core.Tests;

public class ExceptionsTests
{
    [Fact]
    public void DdapClientException_DefaultConstructor_ShouldWork()
    {
        // Act
        var exception = new DdapClientException();

        // Assert
        exception.Should().NotBeNull();
    }

    [Fact]
    public void DdapClientException_MessageConstructor_ShouldWork()
    {
        // Act
        var exception = new DdapClientException("Test message");

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("Test message");
    }

    [Fact]
    public void DdapClientException_MessageAndInnerException_ShouldWork()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new DdapClientException("Test message", innerException);

        // Assert
        exception.Should().NotBeNull();
        exception.Message.Should().Be("Test message");
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void DdapConnectionException_DefaultConstructor_ShouldWork()
    {
        // Act
        var exception = new DdapConnectionException();

        // Assert
        exception.Should().NotBeNull();
        exception.Should().BeAssignableTo<DdapClientException>();
    }

    [Fact]
    public void DdapConnectionException_MessageConstructor_ShouldWork()
    {
        // Act
        var exception = new DdapConnectionException("Connection failed");

        // Assert
        exception.Should().BeAssignableTo<DdapClientException>();
        exception.Message.Should().Be("Connection failed");
    }

    [Fact]
    public void DdapConnectionException_MessageAndInnerException_ShouldWork()
    {
        // Arrange
        var innerException = new System.Net.Http.HttpRequestException("Network error");

        // Act
        var exception = new DdapConnectionException("Connection failed", innerException);

        // Assert
        exception.Should().BeAssignableTo<DdapClientException>();
        exception.Message.Should().Be("Connection failed");
        exception.InnerException.Should().Be(innerException);
    }

    [Fact]
    public void DdapApiException_DefaultConstructor_ShouldWork()
    {
        // Act
        var exception = new DdapApiException();

        // Assert
        exception.Should().NotBeNull();
        exception.Should().BeAssignableTo<DdapClientException>();
        exception.StatusCode.Should().BeNull();
    }

    [Fact]
    public void DdapApiException_MessageConstructor_ShouldWork()
    {
        // Act
        var exception = new DdapApiException("API error");

        // Assert
        exception.Should().BeAssignableTo<DdapClientException>();
        exception.Message.Should().Be("API error");
        exception.StatusCode.Should().BeNull();
    }

    [Fact]
    public void DdapApiException_MessageAndStatusCode_ShouldWork()
    {
        // Act
        var exception = new DdapApiException("API error", 404);

        // Assert
        exception.Should().BeAssignableTo<DdapClientException>();
        exception.Message.Should().Be("API error");
        exception.StatusCode.Should().Be(404);
    }

    [Fact]
    public void DdapApiException_MessageAndInnerException_ShouldWork()
    {
        // Arrange
        var innerException = new InvalidOperationException("Inner error");

        // Act
        var exception = new DdapApiException("API error", innerException);

        // Assert
        exception.Should().BeAssignableTo<DdapClientException>();
        exception.Message.Should().Be("API error");
        exception.InnerException.Should().Be(innerException);
        exception.StatusCode.Should().BeNull();
    }
}
