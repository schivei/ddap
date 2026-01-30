using Ddap.Auth.Policies;
using Ddap.Grpc;
using FluentAssertions;
using Xunit;

namespace Ddap.Tests.RawQuery;

public class QueryAnalyzerTests
{
    [Theory]
    [InlineData("SELECT * FROM Users", QueryType.Select)]
    [InlineData("  SELECT Id FROM Products", QueryType.Select)]
    [InlineData("select name from customers", QueryType.Select)]
    [InlineData("INSERT INTO Users (Name) VALUES ('test')", QueryType.Insert)]
    [InlineData("  insert into products values (1, 'test')", QueryType.Insert)]
    [InlineData("UPDATE Users SET Name = 'test'", QueryType.Update)]
    [InlineData("  update products set price = 10", QueryType.Update)]
    [InlineData("DELETE FROM Users", QueryType.Delete)]
    [InlineData("  delete from products where id = 1", QueryType.Delete)]
    [InlineData("CREATE TABLE Test (Id INT)", QueryType.Create)]
    [InlineData("DROP TABLE Users", QueryType.Drop)]
    [InlineData("ALTER TABLE Users ADD COLUMN Age INT", QueryType.Alter)]
    [InlineData("TRUNCATE TABLE Users", QueryType.Truncate)]
    [InlineData("MERGE INTO Users USING Source ON...", QueryType.Merge)]
    [InlineData("EXEC sp_test", QueryType.Execute)]
    [InlineData("EXECUTE sp_test @param = 1", QueryType.Execute)]
    public void DetermineQueryType_Should_Correctly_Identify_Query_Type(
        string query,
        QueryType expected
    )
    {
        // Act
        var result = QueryAnalyzer.DetermineQueryType(query);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void DetermineQueryType_Should_Return_Unknown_For_Null_Query()
    {
        // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type
        var result = QueryAnalyzer.DetermineQueryType(null);
#pragma warning restore CS8625

        // Assert
        result.Should().Be(QueryType.Unknown);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DetermineQueryType_Should_Return_Unknown_For_Empty_Query(string query)
    {
        // Act
        var result = QueryAnalyzer.DetermineQueryType(query);

        // Assert
        result.Should().Be(QueryType.Unknown);
    }

    [Theory]
    [InlineData("SELECT * FROM Users WHERE Id = @Id", false)]
    [InlineData("SELECT * FROM Users WHERE Name = :name", false)]
    [InlineData("SELECT * FROM Users WHERE Id = $1", false)]
    [InlineData("SELECT * FROM Users", false)]
    public void HasPotentialInjection_Should_Not_Flag_Parameterized_Queries(
        string query,
        bool expectedFlag
    )
    {
        // Act
        var result = QueryAnalyzer.HasPotentialInjection(query);

        // Assert
        result.Should().Be(expectedFlag);
    }

    [Theory]
    [InlineData("SELECT * FROM Users WHERE 1=1 OR 1=1", true)]
    [InlineData("SELECT * FROM Users; DROP TABLE Users;", true)]
    [InlineData("SELECT * FROM Users UNION SELECT * FROM Passwords", true)]
    public void HasPotentialInjection_Should_Flag_Suspicious_Patterns(string query, bool expected)
    {
        // Act
        var result = QueryAnalyzer.HasPotentialInjection(query);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("SELECT * FROM Users", "Users")]
    [InlineData("SELECT * FROM dbo.Users", "dbo.Users")]
    [InlineData("SELECT Id, Name FROM Products WHERE Price > 10", "Products")]
    public void ExtractTableName_Should_Extract_Table_From_Select(string query, string expected)
    {
        // Act
        var result = QueryAnalyzer.ExtractTableName(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain(expected);
    }

    [Theory]
    [InlineData("INSERT INTO Users (Name) VALUES ('test')", "Users")]
    [InlineData("INSERT INTO dbo.Products (Name, Price) VALUES ('item', 10)", "dbo.Products")]
    public void ExtractTableName_Should_Extract_Table_From_Insert(string query, string expected)
    {
        // Act
        var result = QueryAnalyzer.ExtractTableName(query);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("UPDATE Users SET Name = 'test'", "Users")]
    [InlineData("UPDATE dbo.Products SET Price = 10", "dbo.Products")]
    public void ExtractTableName_Should_Extract_Table_From_Update(string query, string expected)
    {
        // Act
        var result = QueryAnalyzer.ExtractTableName(query);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("DELETE FROM Users", "Users")]
    [InlineData("DELETE FROM dbo.Products WHERE Id = 1", "dbo.Products")]
    public void ExtractTableName_Should_Extract_Table_From_Delete(string query, string expected)
    {
        // Act
        var result = QueryAnalyzer.ExtractTableName(query);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("CREATE TABLE Test (Id INT)")]
    [InlineData("DROP TABLE Users")]
    [InlineData("TRUNCATE TABLE Products")]
    public void ExtractTableName_Should_Return_Null_For_Unsupported_Queries(string query)
    {
        // Act
        var result = QueryAnalyzer.ExtractTableName(query);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void ExtractTableName_Should_Return_Null_For_Empty_Query()
    {
        // Act
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type
        var result = QueryAnalyzer.ExtractTableName(null);
#pragma warning restore CS8625

        // Assert
        result.Should().BeNull();
    }
}
