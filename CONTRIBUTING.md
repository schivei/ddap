# Contributing to DDAP

Thank you for considering contributing to DDAP! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Setup](#development-setup)
- [Pull Request Process](#pull-request-process)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Documentation](#documentation)

## Code of Conduct

### Our Pledge

We are committed to providing a welcoming and inclusive environment for all contributors. We expect all participants to:

- Use welcoming and inclusive language
- Be respectful of differing viewpoints and experiences
- Gracefully accept constructive criticism
- Focus on what is best for the community
- Show empathy towards other community members

### Unacceptable Behavior

- Harassment, trolling, or discriminatory comments
- Publishing others' private information without permission
- Other conduct which could reasonably be considered inappropriate

## How Can I Contribute?

### Reporting Bugs

Before submitting a bug report:

1. **Check existing issues** - Someone might have already reported it
2. **Verify it's reproducible** - Make sure the bug consistently occurs
3. **Check the documentation** - The behavior might be expected

When submitting a bug report, include:

- **Clear title** describing the issue
- **Step-by-step reproduction** instructions
- **Expected vs actual behavior**
- **Environment details**:
  - DDAP version
  - .NET version
  - Database type and version
  - Operating system
- **Code samples** or repository link demonstrating the issue
- **Error messages and stack traces**

**Template:**
```markdown
## Bug Description
A clear description of the bug.

## Steps to Reproduce
1. Install package X
2. Configure with Y
3. Run application
4. Error occurs

## Expected Behavior
What you expected to happen.

## Actual Behavior
What actually happened.

## Environment
- DDAP Version: 1.0.0
- .NET Version: 10.0
- Database: SQL Server 2022
- OS: Windows 11

## Additional Context
Any other relevant information, logs, or screenshots.
```

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When suggesting:

1. **Use a clear and descriptive title**
2. **Provide detailed description** of the proposed feature
3. **Explain why this enhancement would be useful**
4. **Provide examples** of how it would work
5. **Reference similar features** in other projects if applicable

**Template:**
```markdown
## Feature Description
A clear description of the proposed feature.

## Use Case
Why this feature would be useful and what problem it solves.

## Proposed Solution
How you envision this feature working.

## Alternatives Considered
Other approaches you've thought about.

## Additional Context
Mockups, diagrams, or examples.
```

### Contributing Code

1. **Fork the repository**
2. **Create a feature branch** from `main`
3. **Make your changes**
4. **Write or update tests**
5. **Update documentation**
6. **Submit a pull request**

## Development Setup

### Prerequisites

- **.NET 10 SDK** or later
- **Git**
- **Docker** (for running database containers in tests)
- **Code editor** (Visual Studio 2022, VS Code, or Rider)

### Getting Started

1. **Clone your fork:**
   ```bash
   git clone https://github.com/YOUR-USERNAME/ddap.git
   cd ddap
   ```

2. **Add upstream remote:**
   ```bash
   git remote add upstream https://github.com/schivei/ddap.git
   ```

3. **Install dependencies:**
   ```bash
   dotnet restore
   ```

4. **Build the solution:**
   ```bash
   dotnet build
   ```

5. **Run tests:**
   ```bash
   dotnet test
   ```

### Project Structure

```
ddap/
├── src/                          # Source code
│   ├── Ddap.Core/               # Core abstractions
│   ├── Ddap.Data.Dapper.*/      # Dapper providers
│   ├── Ddap.Data.EntityFramework/ # EF provider
│   ├── Ddap.Rest/               # REST API provider
│   ├── Ddap.GraphQL/            # GraphQL provider
│   ├── Ddap.Grpc/               # gRPC provider
│   └── Ddap.*/                  # Other packages
├── tests/                       # Unit and integration tests
│   └── Ddap.Tests/
├── examples/                    # Example applications
│   └── Ddap.Example.Api/
├── docs/                        # Documentation
└── .github/                     # GitHub workflows
```

### Running Database Tests

Tests require database containers. Start them with:

```bash
# SQL Server
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
  -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# MySQL
docker run -e MYSQL_ROOT_PASSWORD=root -e MYSQL_DATABASE=testdb \
  -p 3306:3306 --name mysql -d mysql:8.0

# PostgreSQL
docker run -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=testdb \
  -p 5432:5432 --name postgres -d postgres:16
```

Run specific test categories:

```bash
# Unit tests only
dotnet test --filter Category=Unit

# Integration tests
dotnet test --filter Category=Integration

# SQL Server tests
dotnet test --filter Category=SqlServer
```

## Pull Request Process

### Before Submitting

1. **Update from main:**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Run all tests:**
   ```bash
   dotnet test
   ```

3. **Run code formatting:**
   ```bash
   dotnet format
   ```

4. **Update documentation** if needed

5. **Add tests** for new features

### PR Guidelines

1. **Create descriptive PR title:**
   - `feat:` for new features
   - `fix:` for bug fixes
   - `docs:` for documentation changes
   - `refactor:` for code refactoring
   - `test:` for test additions/changes
   - `chore:` for maintenance tasks

   Examples:
   - `feat: Add PostgreSQL array type support`
   - `fix: Handle null foreign keys correctly`
   - `docs: Update REST API examples`

2. **Provide detailed description:**
   ```markdown
   ## Description
   Brief description of the changes.

   ## Motivation
   Why are these changes needed?

   ## Changes Made
   - Added X
   - Modified Y
   - Removed Z

   ## Testing
   - Added unit tests for feature X
   - Verified manually with example app
   
   ## Breaking Changes
   None / List any breaking changes

   ## Screenshots
   (if applicable)
   ```

3. **Link related issues:**
   ```markdown
   Fixes #123
   Relates to #456
   ```

4. **Request review** from maintainers

### PR Checklist

- [ ] Code follows project coding standards
- [ ] All tests pass
- [ ] New tests added for new functionality
- [ ] Documentation updated
- [ ] No breaking changes (or documented if necessary)
- [ ] Commit messages are clear and descriptive
- [ ] Branch is up-to-date with main

## Coding Standards

### General Principles

- **Write clean, readable code** - Code is read more than written
- **Follow SOLID principles** - Especially Single Responsibility
- **Keep methods small** - Ideally under 20 lines
- **Use meaningful names** - Variables, methods, and classes should be self-documenting
- **Add comments sparingly** - Code should be self-explanatory; use comments for "why" not "what"

### C# Conventions

Follow the [.NET coding conventions](https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions):

```csharp
// ✅ Good
public class EntityConfiguration : IEntityConfiguration
{
    private readonly string _name;
    
    public EntityConfiguration(string name)
    {
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }
    
    public string Name => _name;
}

// ❌ Bad
public class entityconfiguration
{
    public string name;
}
```

### Code Formatting

The project uses **CSharpier** for consistent code formatting. The formatting is enforced through:

1. **Pre-commit hooks** - Automatically formats code before commits
2. **CI/CD pipeline** - Validates formatting in pull requests
3. **Build integration** - Auto-formats in Debug builds

#### Manual Formatting Commands

```bash
# Restore .NET tools (first time or after clean)
dotnet tool restore

# Format all files in the solution
dotnet csharpier format .

# Check if formatting is needed (without modifying files)
dotnet csharpier check .
```

#### Pre-commit Hook

The project uses **Husky.Net** for git hooks. After cloning the repository:

```bash
# Restore .NET tools (includes Husky.Net and CSharpier)
dotnet tool restore

# Install git hooks
dotnet husky install
```

The pre-commit hook will automatically:
1. Restore .NET tools
2. Format code with CSharpier
3. Auto-stage formatted files
4. Run unit tests
5. Abort commit only if tests fail

#### Editor Integration

Configure your IDE to use CSharpier:

- **Visual Studio**: Install the CSharpier extension
- **VS Code**: Install the "CSharpier - Code formatter" extension
- **JetBrains Rider**: Install the CSharpier plugin

Configuration is in `.editorconfig` and `.csharpierrc.json`.

### Naming Conventions

- **Classes:** PascalCase - `EntityRepository`
- **Interfaces:** PascalCase with 'I' prefix - `IEntityConfiguration`
- **Methods:** PascalCase - `GetEntity`
- **Properties:** PascalCase - `EntityName`
- **Fields (private):** camelCase with '_' prefix - `_repository`
- **Parameters:** camelCase - `entityName`
- **Constants:** PascalCase - `DefaultTimeout`

### Async/Await

- Always use `async/await` for I/O operations
- Suffix async methods with `Async`
- Use `ConfigureAwait(false)` in library code

```csharp
public async Task<IEntityConfiguration> LoadEntityAsync(string name)
{
    var data = await _dataProvider.GetMetadataAsync(name)
        .ConfigureAwait(false);
    return CreateEntityConfiguration(data);
}
```

## Testing Guidelines

### Test Structure

- **Arrange-Act-Assert** pattern
- One assertion per test (when practical)
- Clear test names describing the scenario

```csharp
[Fact]
public async Task LoadEntitiesAsync_WhenDatabaseHasTables_ReturnsEntities()
{
    // Arrange
    var provider = new SqlServerDapperProvider(_connectionString);
    var repository = new EntityRepository();

    // Act
    await provider.LoadEntitiesAsync(repository);

    // Assert
    Assert.NotEmpty(repository.Entities);
}
```

### Test Naming

```csharp
// Pattern: MethodName_Scenario_ExpectedResult

[Fact]
public void GetEntity_WhenEntityExists_ReturnsEntity() { }

[Fact]
public void GetEntity_WhenEntityDoesNotExist_ReturnsNull() { }

[Fact]
public void AddEntity_WhenEntityIsNull_ThrowsArgumentNullException() { }
```

### Test Categories

Use categories to organize tests:

```csharp
[Fact]
[Trait("Category", "Unit")]
public void UnitTest() { }

[Fact]
[Trait("Category", "Integration")]
public void IntegrationTest() { }

[Fact]
[Trait("Category", "SqlServer")]
public void SqlServerTest() { }
```

### Mocking

Use Moq for mocking dependencies:

```csharp
[Fact]
public async Task Controller_CallsRepository()
{
    // Arrange
    var mockRepository = new Mock<IEntityRepository>();
    mockRepository.Setup(r => r.GetEntity(It.IsAny<string>()))
                  .Returns(new EntityConfiguration());
    
    var controller = new EntityController(mockRepository.Object);

    // Act
    var result = await controller.GetMetadata("TestEntity");

    // Assert
    mockRepository.Verify(r => r.GetEntity("TestEntity"), Times.Once);
}
```

## Documentation

### Code Documentation

- Add XML comments for public APIs
- Document parameters and return values
- Include usage examples for complex features

```csharp
/// <summary>
/// Loads entity metadata from the database.
/// </summary>
/// <param name="repository">The repository to populate with entities.</param>
/// <returns>A task representing the asynchronous operation.</returns>
/// <exception cref="ArgumentNullException">Thrown when repository is null.</exception>
/// <example>
/// <code>
/// var repository = new EntityRepository();
/// await provider.LoadEntitiesAsync(repository);
/// </code>
/// </example>
public async Task LoadEntitiesAsync(IEntityRepository repository)
{
    // Implementation
}
```

### Updating Documentation

When making changes:

1. **Update README.md** if affecting main features
2. **Update docs/** for detailed changes
3. **Update code examples** to reflect new patterns
4. **Add migration guides** for breaking changes

### Documentation Style

- Use clear, concise language
- Include code examples
- Add diagrams for complex concepts
- Link to related documentation

## Getting Help

- **Questions:** Open a [discussion](https://github.com/schivei/ddap/discussions)
- **Bugs:** Open an [issue](https://github.com/schivei/ddap/issues)
- **Features:** Open an [issue](https://github.com/schivei/ddap/issues) with enhancement label

## Recognition

Contributors will be:
- Listed in release notes
- Acknowledged in the repository
- Thanked in commit messages

Thank you for contributing to DDAP! 🎉
