using Ddap.Core;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ddap.IntegrationTests;

public class EntityRepositoryIntegrationTests : IDisposable
{
    private readonly TestDbContext _context;
    private readonly DbSet<TestEntity> _dbSet;

    public EntityRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestDbContext(options);
        _dbSet = _context.TestEntities;
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Entity",
            Value = 42,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };

        // Act
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Assert
        var savedEntity = await _context.TestEntities.FindAsync(entity.Id);
        Assert.NotNull(savedEntity);
        Assert.Equal("Test Entity", savedEntity.Name);
        Assert.Equal(42, savedEntity.Value);
        Assert.True(savedEntity.IsActive);
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAddMultipleEntities()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 1",
                Value = 1,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 2",
                Value = 2,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 3",
                Value = 3,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
        };

        // Act
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Assert
        var count = await _context.TestEntities.CountAsync();
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Entity",
            Value = 100,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _dbSet.FindAsync(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(entity.Id, result.Id);
        Assert.Equal("Test Entity", result.Name);
        Assert.Equal(100, result.Value);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _dbSet.FindAsync("non-existent-id");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 1",
                Value = 1,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 2",
                Value = 2,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 3",
                Value = 3,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
        };
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _dbSet.ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Original Name",
            Value = 10,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Name = "Updated Name";
        entity.Value = 20;
        entity.IsActive = false;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.TestEntities.FindAsync(entity.Id);
        Assert.NotNull(updated);
        Assert.Equal("Updated Name", updated.Name);
        Assert.Equal(20, updated.Value);
        Assert.False(updated.IsActive);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            Name = "To Delete",
            Value = 99,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _context.TestEntities.FindAsync(entity.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnMatchingEntities()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Active 1",
                Value = 100,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Active 2",
                Value = 200,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Inactive",
                Value = 300,
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
            },
        };
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var result = await _dbSet.Where(e => e.IsActive).ToListAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, e => Assert.True(e.IsActive));
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 1",
                Value = 1,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 2",
                Value = 2,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Entity 3",
                Value = 3,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
        };
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var count = await _dbSet.CountAsync();

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnMatchingCount()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Active 1",
                Value = 100,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Active 2",
                Value = 200,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Inactive",
                Value = 300,
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
            },
        };
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var activeCount = await _dbSet.CountAsync(e => e.IsActive);
        var inactiveCount = await _dbSet.CountAsync(e => !e.IsActive);

        // Assert
        Assert.Equal(2, activeCount);
        Assert.Equal(1, inactiveCount);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenEntitiesExist()
    {
        // Arrange
        var entity = new TestEntity
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test",
            Value = 1,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
        };
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        var result = await _dbSet.AnyAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnFalse_WhenNoEntitiesExist()
    {
        // Act
        var result = await _dbSet.AnyAsync();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AnyAsync_WithPredicate_ShouldReturnCorrectValue()
    {
        // Arrange
        var entities = new[]
        {
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Active",
                Value = 100,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
            },
            new TestEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Inactive",
                Value = 200,
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
            },
        };
        await _dbSet.AddRangeAsync(entities);
        await _context.SaveChangesAsync();

        // Act
        var hasActive = await _dbSet.AnyAsync(e => e.IsActive);
        var hasHighValue = await _dbSet.AnyAsync(e => e.Value > 1000);

        // Assert
        Assert.True(hasActive);
        Assert.False(hasHighValue);
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
