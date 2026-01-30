using Ddap.Auth.Policies;
using FluentAssertions;
using Xunit;

namespace Ddap.Tests.RawQuery;

public class RawQueryPolicyTests
{
    [Fact]
    public async Task DefaultRawQueryPolicy_Should_Allow_Select_Queries()
    {
        // Arrange
        var policy = new DefaultRawQueryPolicy();
        var context = new RawQueryContext
        {
            Query = "SELECT * FROM Users",
            QueryType = QueryType.Select,
        };

        // Act
        var result = await policy.CanExecuteQueryAsync(context);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DefaultRawQueryPolicy_Should_Deny_Insert_Queries()
    {
        // Arrange
        var policy = new DefaultRawQueryPolicy();
        var context = new RawQueryContext
        {
            Query = "INSERT INTO Users (Name) VALUES ('test')",
            QueryType = QueryType.Insert,
        };

        // Act
        var result = await policy.CanExecuteQueryAsync(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DefaultRawQueryPolicy_Should_Deny_Update_Queries()
    {
        // Arrange
        var policy = new DefaultRawQueryPolicy();
        var context = new RawQueryContext
        {
            Query = "UPDATE Users SET Name = 'test'",
            QueryType = QueryType.Update,
        };

        // Act
        var result = await policy.CanExecuteQueryAsync(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DefaultRawQueryPolicy_Should_Deny_Delete_Queries()
    {
        // Arrange
        var policy = new DefaultRawQueryPolicy();
        var context = new RawQueryContext
        {
            Query = "DELETE FROM Users",
            QueryType = QueryType.Delete,
        };

        // Act
        var result = await policy.CanExecuteQueryAsync(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DefaultRawQueryPolicy_Should_Deny_Drop_Queries()
    {
        // Arrange
        var policy = new DefaultRawQueryPolicy();
        var context = new RawQueryContext
        {
            Query = "DROP TABLE Users",
            QueryType = QueryType.Drop,
        };

        // Act
        var result = await policy.CanExecuteQueryAsync(context);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AllowAllRawQueryPolicy_Should_Allow_All_Queries()
    {
        // Arrange
        var policy = new AllowAllRawQueryPolicy();
        var contexts = new[]
        {
            new RawQueryContext { Query = "SELECT * FROM Users", QueryType = QueryType.Select },
            new RawQueryContext
            {
                Query = "INSERT INTO Users (Name) VALUES ('test')",
                QueryType = QueryType.Insert,
            },
            new RawQueryContext
            {
                Query = "UPDATE Users SET Name = 'test'",
                QueryType = QueryType.Update,
            },
            new RawQueryContext { Query = "DELETE FROM Users", QueryType = QueryType.Delete },
            new RawQueryContext { Query = "DROP TABLE Users", QueryType = QueryType.Drop },
        };

        // Act & Assert
        foreach (var context in contexts)
        {
            var result = await policy.CanExecuteQueryAsync(context);
            result.Should().BeTrue($"AllowAll should allow {context.QueryType}");
        }
    }

    [Fact]
    public async Task DenyAllRawQueryPolicy_Should_Deny_All_Queries()
    {
        // Arrange
        var policy = new DenyAllRawQueryPolicy();
        var contexts = new[]
        {
            new RawQueryContext { Query = "SELECT * FROM Users", QueryType = QueryType.Select },
            new RawQueryContext
            {
                Query = "INSERT INTO Users (Name) VALUES ('test')",
                QueryType = QueryType.Insert,
            },
            new RawQueryContext
            {
                Query = "UPDATE Users SET Name = 'test'",
                QueryType = QueryType.Update,
            },
            new RawQueryContext { Query = "DELETE FROM Users", QueryType = QueryType.Delete },
        };

        // Act & Assert
        foreach (var context in contexts)
        {
            var result = await policy.CanExecuteQueryAsync(context);
            result.Should().BeFalse($"DenyAll should deny {context.QueryType}");
        }
    }

    [Fact]
    public void RawQueryContext_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var context = new RawQueryContext
        {
            Query = "SELECT * FROM Users",
            QueryType = QueryType.Select,
            DatabaseName = "TestDb",
            TableName = "Users",
            UserId = "user123",
            UserRoles = new[] { "admin", "user" },
        };

        // Assert
        context.Query.Should().Be("SELECT * FROM Users");
        context.QueryType.Should().Be(QueryType.Select);
        context.DatabaseName.Should().Be("TestDb");
        context.TableName.Should().Be("Users");
        context.UserId.Should().Be("user123");
        context.UserRoles.Should().Contain("admin").And.Contain("user");
    }

    [Fact]
    public void QueryType_Enum_Should_Have_All_Expected_Values()
    {
        // Assert
        Enum.GetValues(typeof(QueryType))
            .Cast<QueryType>()
            .Should()
            .Contain(
                new[]
                {
                    QueryType.Unknown,
                    QueryType.Select,
                    QueryType.Insert,
                    QueryType.Update,
                    QueryType.Delete,
                    QueryType.Create,
                    QueryType.Drop,
                    QueryType.Alter,
                    QueryType.Truncate,
                    QueryType.Merge,
                    QueryType.Execute,
                }
            );
    }
}
