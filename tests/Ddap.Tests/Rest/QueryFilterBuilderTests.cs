using Ddap.Rest.Filters;
using FluentAssertions;
using Xunit;

namespace Ddap.Tests.Rest;

public class QueryFilterBuilderTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UserId { get; set; }
    }

    [Fact]
    public void ApplyFilter_Should_Return_Unfiltered_Query_When_Expression_Is_Null()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new()
            {
                Id = 1,
                Name = "John",
                Age = 30,
            },
            new()
            {
                Id = 2,
                Name = "Jane",
                Age = 25,
            },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, null!);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyFilter_Should_Return_Unfiltered_Query_When_Expression_Is_Empty()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "   ");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_Equals_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new()
            {
                Id = 1,
                Name = "John",
                Age = 30,
            },
            new()
            {
                Id = 2,
                Name = "Jane",
                Age = 25,
            },
            new()
            {
                Id = 3,
                Name = "Bob",
                Age = 30,
            },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Age eq 30");

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Name).Should().Contain(new[] { "John", "Bob" });
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_NotEquals_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new()
            {
                Id = 1,
                Name = "John",
                Status = "active",
            },
            new()
            {
                Id = 2,
                Name = "Jane",
                Status = "inactive",
            },
            new()
            {
                Id = 3,
                Name = "Bob",
                Status = "active",
            },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Status ne 'active'");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Jane");
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_GreaterThan_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 1, Age = 20 },
            new() { Id = 2, Age = 30 },
            new() { Id = 3, Age = 40 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Age gt 25");

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Age).Should().Contain(new[] { 30, 40 });
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_GreaterThanOrEqual_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 1, Age = 20 },
            new() { Id = 2, Age = 30 },
            new() { Id = 3, Age = 40 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Age ge 30");

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Age).Should().Contain(new[] { 30, 40 });
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_LessThan_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 1, Age = 20 },
            new() { Id = 2, Age = 30 },
            new() { Id = 3, Age = 40 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Age lt 35");

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Age).Should().Contain(new[] { 20, 30 });
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_LessThanOrEqual_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 1, Age = 20 },
            new() { Id = 2, Age = 30 },
            new() { Id = 3, Age = 40 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Age le 30");

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Age).Should().Contain(new[] { 20, 30 });
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_Contains_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 1, Name = "John Smith" },
            new() { Id = 2, Name = "Jane Doe" },
            new() { Id = 3, Name = "Bob Smith" },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Name contains 'Smith'");

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Name).Should().Contain(new[] { "John Smith", "Bob Smith" });
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_And_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new()
            {
                Name = "John",
                Age = 30,
                Status = "active",
            },
            new()
            {
                Name = "Jane",
                Age = 25,
                Status = "active",
            },
            new()
            {
                Name = "Bob",
                Age = 30,
                Status = "inactive",
            },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Age eq 30 and Status eq 'active'");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("John");
    }

    [Fact]
    public void ApplyFilter_Should_Filter_With_Or_Operator()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Name = "John", Age = 30 },
            new() { Name = "Jane", Age = 25 },
            new() { Name = "Bob", Age = 35 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Age eq 30 or Age eq 35");

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Name).Should().Contain(new[] { "John", "Bob" });
    }

    [Fact]
    public void ApplyFilter_Should_Handle_Complex_Expression_With_And_Or()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new()
            {
                Name = "John",
                Age = 30,
                Status = "active",
            },
            new()
            {
                Name = "Jane",
                Age = 25,
                Status = "active",
            },
            new()
            {
                Name = "Bob",
                Age = 30,
                Status = "inactive",
            },
            new()
            {
                Name = "Alice",
                Age = 35,
                Status = "active",
            },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Age gt 28 and Status eq 'active'");

        // Assert
        result.Should().HaveCount(2);
        result.Select(e => e.Name).Should().Contain(new[] { "John", "Alice" });
    }

    [Fact]
    public void ApplyFilter_Should_Handle_Invalid_Expression_Gracefully()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "invalid expression here");

        // Assert - Should return unfiltered query on error
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyFilter_Should_Handle_NonExistent_Property()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 1, Name = "John" },
            new() { Id = 2, Name = "Jane" },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "NonExistentProperty eq 'value'");

        // Assert - Should return unfiltered query
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyFilter_Should_Handle_String_Values_With_Quotes()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Name = "John" },
            new() { Name = "Jane" },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Name eq \"John\"");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("John");
    }

    [Fact]
    public void ApplyFilter_Should_Work_With_Decimal_Type()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Salary = 50000m },
            new() { Salary = 75000m },
            new() { Salary = 100000m },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "Salary gt 60000");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyFilter_Should_Work_With_Boolean_Type()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { IsActive = true, Name = "John" },
            new() { IsActive = false, Name = "Jane" },
            new() { IsActive = true, Name = "Bob" },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, "IsActive eq true");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyFilter_Should_Work_With_DateTime_Type()
    {
        // Arrange
        var date1 = new DateTime(2024, 1, 1);
        var date2 = new DateTime(2024, 6, 1);
        var date3 = new DateTime(2024, 12, 1);

        var query = new List<TestEntity>
        {
            new() { CreatedDate = date1 },
            new() { CreatedDate = date2 },
            new() { CreatedDate = date3 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, $"CreatedDate gt {date1:yyyy-MM-dd}");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyFilter_Should_Work_With_Guid_Type()
    {
        // Arrange
        var guid1 = Guid.NewGuid();
        var guid2 = Guid.NewGuid();

        var query = new List<TestEntity>
        {
            new() { UserId = guid1 },
            new() { UserId = guid2 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyFilter(query, $"UserId eq '{guid1}'");

        // Assert
        result.Should().HaveCount(1);
        result.First().UserId.Should().Be(guid1);
    }

    [Fact]
    public void ApplyOrdering_Should_Return_Unfiltered_Query_When_Expression_Is_Null()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 2, Name = "Jane" },
            new() { Id = 1, Name = "John" },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, null!);

        // Assert
        result.Should().HaveCount(2);
        result.First().Id.Should().Be(2); // Original order preserved
    }

    [Fact]
    public void ApplyOrdering_Should_Return_Unfiltered_Query_When_Expression_Is_Empty()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 2, Name = "Jane" },
            new() { Id = 1, Name = "John" },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, "   ");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyOrdering_Should_Sort_Ascending_By_Default()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Name = "Charlie", Age = 30 },
            new() { Name = "Alice", Age = 25 },
            new() { Name = "Bob", Age = 35 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, "Name");

        // Assert
        result.Select(e => e.Name).Should().ContainInOrder("Alice", "Bob", "Charlie");
    }

    [Fact]
    public void ApplyOrdering_Should_Sort_Ascending_Explicitly()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Age = 30 },
            new() { Age = 25 },
            new() { Age = 35 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, "Age asc");

        // Assert
        result.Select(e => e.Age).Should().ContainInOrder(25, 30, 35);
    }

    [Fact]
    public void ApplyOrdering_Should_Sort_Descending()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Age = 30 },
            new() { Age = 25 },
            new() { Age = 35 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, "Age desc");

        // Assert
        result.Select(e => e.Age).Should().ContainInOrder(35, 30, 25);
    }

    [Fact]
    public void ApplyOrdering_Should_Support_Multiple_Sort_Fields()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new()
            {
                Status = "active",
                Age = 30,
                Name = "John",
            },
            new()
            {
                Status = "active",
                Age = 25,
                Name = "Jane",
            },
            new()
            {
                Status = "inactive",
                Age = 30,
                Name = "Bob",
            },
            new()
            {
                Status = "active",
                Age = 30,
                Name = "Alice",
            },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, "Status asc, Age desc, Name asc");

        // Assert
        var resultList = result.ToList();
        resultList.Should().HaveCount(4);

        // First item: status=active, age=30, name=Alice
        resultList[0].Status.Should().Be("active");
        resultList[0].Age.Should().Be(30);
        resultList[0].Name.Should().Be("Alice");

        // Second item: status=active, age=30, name=John
        resultList[1].Status.Should().Be("active");
        resultList[1].Age.Should().Be(30);
        resultList[1].Name.Should().Be("John");

        // Third item: status=active, age=25, name=Jane
        resultList[2].Status.Should().Be("active");
        resultList[2].Age.Should().Be(25);
        resultList[2].Name.Should().Be("Jane");

        // Fourth item: status=inactive, age=30, name=Bob
        resultList[3].Status.Should().Be("inactive");
    }

    [Fact]
    public void ApplyOrdering_Should_Handle_NonExistent_Property_Gracefully()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Id = 2 },
            new() { Id = 1 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, "NonExistentProperty asc");

        // Assert - Should return unmodified query
        result.Should().HaveCount(2);
    }

    [Fact]
    public void ApplyOrdering_Should_Handle_Case_Insensitive_Property_Names()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Age = 30 },
            new() { Age = 25 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, "age asc");

        // Assert
        result.Select(e => e.Age).Should().ContainInOrder(25, 30);
    }

    [Fact]
    public void ApplyOrdering_Should_Handle_Mixed_Ascending_And_Descending()
    {
        // Arrange
        var query = new List<TestEntity>
        {
            new() { Status = "active", Age = 30 },
            new() { Status = "inactive", Age = 25 },
            new() { Status = "active", Age = 35 },
        }.AsQueryable();

        // Act
        var result = QueryFilterBuilder.ApplyOrdering(query, "Status asc, Age desc");

        // Assert
        var resultList = result.ToList();
        resultList[0].Status.Should().Be("active");
        resultList[0].Age.Should().Be(35); // Within "active", descending age
    }
}
