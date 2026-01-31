# GitHub Copilot Instructions for DDAP Project

## Project Overview

**DDAP** (Database-Driven API Provider) is a .NET library that empowers developers to create data-driven APIs with **developer control** at the core. The project philosophy is centered on providing infrastructure without imposing opinions, allowing developers to make their own architectural decisions.

## Core Philosophy: "Developer in Control"

### Key Principles

1. **No Forced Dependencies**: Never force specific implementations. Provide choices and let developers decide.
2. **Explicit Over Implicit**: Everything should be transparent and controllable.
3. **Minimal Abstractions**: Only abstract what needs to be abstracted.
4. **Official Packages First**: Prefer official, vendor-supported packages over community alternatives.
5. **Zero Opinions**: DDAP provides infrastructure, not architecture.

### What This Means in Practice

- ❌ DON'T: Force Pomelo.EntityFrameworkCore.MySql (community package)
- ✅ DO: Use MySql.EntityFrameworkCore (official Oracle package) or let user choose
- ❌ DON'T: Hide database connection configuration
- ✅ DO: Make all configuration explicit and overridable
- ❌ DON'T: Impose a specific repository pattern
- ✅ DO: Provide tools that work with any pattern

## Code Quality Standards

### Before Every Commit

**CRITICAL**: Always run CSharpier before committing:

```bash
# Restore tools (first time)
dotnet tool restore

# Format all files
dotnet csharpier .

# Or use the pre-commit hook (auto-installed with Husky)
git commit -m "your message"  # Hook runs automatically
```

**The pre-commit hook will:**
1. Format code with CSharpier
2. Run unit tests
3. Abort if tests fail

### Code Formatting Rules

- Use CSharpier for ALL C# code formatting
- Follow `.editorconfig` settings
- No manual spacing adjustments
- Let the formatter handle style

### Coding Standards

```csharp
// ✅ GOOD: Clear, explicit, testable
public class EntityRepository : IEntityRepository
{
    private readonly IDbConnection _connection;
    
    public EntityRepository(IDbConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }
    
    public async Task<Entity?> GetByIdAsync(int id)
    {
        const string sql = "SELECT * FROM Entities WHERE Id = @Id";
        return await _connection.QueryFirstOrDefaultAsync<Entity>(sql, new { Id = id })
            .ConfigureAwait(false);
    }
}

// ❌ BAD: Hidden dependencies, no null checks, synchronous
public class EntityRepository
{
    public Entity GetById(int id)
    {
        using var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["Default"]);
        return connection.QueryFirstOrDefault<Entity>("SELECT * FROM Entities WHERE Id = " + id);
    }
}
```

### Async/Await Guidelines

- ALL I/O operations MUST be async
- Use `ConfigureAwait(false)` in library code
- Suffix async methods with `Async`
- Never use `.Result` or `.Wait()` - causes deadlocks

## Commit Message Standards

Follow Conventional Commits format:

```
<type>(<scope>): <description>

[optional body]

[optional footer]
```

### Types

- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `refactor:` - Code refactoring (no behavior change)
- `test:` - Adding or updating tests
- `chore:` - Maintenance (dependencies, tooling)
- `perf:` - Performance improvement
- `ci:` - CI/CD changes

### Examples

```bash
# Good commits
git commit -m "feat(rest): add support for custom route prefixes"
git commit -m "fix(dapper): handle null foreign keys correctly"
git commit -m "docs: update MySQL provider setup guide"
git commit -m "refactor(core): simplify entity configuration builder"
git commit -m "test(graphql): add integration tests for mutations"
git commit -m "chore(deps): update Entity Framework to 9.0"

# Bad commits (don't do this)
git commit -m "fixed stuff"
git commit -m "WIP"
git commit -m "updates"
```

### Commit Body (when needed)

```
feat(auth): add JWT authentication support

Implements JWT bearer token authentication for REST APIs.
Includes token validation, refresh token support, and 
configurable token expiration.

Closes #123
```

## Working with the Codebase

### When Making Changes

1. **Understand the Philosophy**: Does this change respect "Developer in Control"?
2. **Check Existing Patterns**: Follow established patterns in the codebase
3. **Format Code**: Run CSharpier before committing
4. **Run Tests**: Ensure all tests pass
5. **Update Documentation**: Keep docs in sync with code
6. **Provide Evidence**: Include test results, screenshots, or examples

### Including Evidence

When working on features (not meta-tasks like this documentation):

```markdown
## Evidence

### Before
[Screenshot or code snippet showing previous behavior]

### After
[Screenshot or code snippet showing new behavior]

### Test Results
```bash
dotnet test
# Output showing passing tests
```

### Example Usage
```csharp
// Code example demonstrating the feature
```
```

### Package Version Strategy

Always use **major version wildcards** for auto-updates:

```xml
<!-- ✅ GOOD: Auto-updates patches and minor versions -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.*" />
<PackageReference Include="Npgsql" Version="8.*" />

<!-- ❌ BAD: Locked to specific patch version -->
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.0" />
```

### Official Packages Only

Prefer official, vendor-supported packages:

- SQL Server: `Microsoft.Data.SqlClient` (Microsoft)
- MySQL: `MySql.Data` or `MySql.EntityFrameworkCore` (Oracle)
- PostgreSQL: `Npgsql` (official)
- SQLite: `Microsoft.Data.Sqlite` (Microsoft)

**Exception**: MySqlConnector for Dapper (performance reasons, documented)

## Testing Requirements

### Test Every Change

- **Unit Tests**: For business logic
- **Integration Tests**: For database operations
- **Examples**: Update example apps to demonstrate features

### Test Structure

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedResult()
{
    // Arrange
    var sut = new SystemUnderTest();
    
    // Act
    var result = await sut.DoSomethingAsync();
    
    // Assert
    Assert.NotNull(result);
}
```

### Test Categories

```csharp
[Trait("Category", "Unit")]      // Fast, no external dependencies
[Trait("Category", "Integration")] // Requires database
[Trait("Category", "SqlServer")]  // SQL Server specific
```

## Documentation Requirements

### Update Documentation When

- Adding new features
- Changing existing behavior
- Fixing bugs that affect usage
- Adding configuration options

### Documentation Files

- `README.md` - Main project overview and quick start
- `docs/*.md` - Detailed feature documentation
- Code XML comments - Public API documentation
- Examples - Working code in `examples/` folder

## Cleaning Up Temporary Files

### Orphan Markdown Files

The project may accumulate analysis/planning MD files. **Always clean these up**:

```bash
# Move to docs/analysis/ or delete if no longer needed
mv ANALYSIS_*.md docs/analysis/ 2>/dev/null || true
mv SPRINT*_*.md docs/sprints/ 2>/dev/null || true
mv TEST_*.md docs/testing/ 2>/dev/null || true

# Or delete truly temporary files
rm -f TEMP_*.md WIP_*.md
```

### What to Keep

- `README.md` - Project main documentation
- `CONTRIBUTING.md` - Contribution guidelines
- `CODE_OF_CONDUCT.md` - Community standards
- `SECURITY.md` - Security policy
- `LICENSE` - Project license
- `docs/**/*.md` - Organized documentation

### What to Remove/Move

- Analysis documents (move to `docs/analysis/`)
- Sprint/planning documents (move to `docs/sprints/`)
- Test reports (move to `docs/testing/`)
- Temporary notes/WIP documents (delete)

## Security Considerations

### Never Commit

- Secrets, API keys, passwords
- Connection strings with credentials
- Personal information
- Temporary test data with sensitive info

### Use Secure Patterns

```csharp
// ✅ GOOD: Parameterized queries
const string sql = "SELECT * FROM Users WHERE Email = @Email";
var user = await connection.QueryFirstOrDefaultAsync<User>(sql, new { Email = email });

// ❌ BAD: SQL injection vulnerability
var sql = $"SELECT * FROM Users WHERE Email = '{email}'";
var user = await connection.QueryFirstOrDefaultAsync<User>(sql);
```

## Common Pitfalls to Avoid

1. **Breaking Philosophy**: Adding forced dependencies or hidden configurations
2. **Skipping CSharpier**: Code style inconsistencies
3. **Ignoring Tests**: Breaking existing functionality
4. **Poor Commit Messages**: Hard to understand history
5. **Missing Documentation**: Users don't know how to use features
6. **Synchronous I/O**: Performance issues and potential deadlocks
7. **Wrong Package Versions**: Using non-existent or incompatible versions

## Project Structure

```
ddap/
├── .github/
│   ├── workflows/          # CI/CD pipelines
│   ├── copilot-instructions.md
│   └── dependabot.yml
├── src/
│   ├── Ddap.Core/         # Core abstractions
│   ├── Ddap.Data.*/       # Data access providers
│   └── Ddap.*/            # Feature packages
├── tests/
│   └── Ddap.Tests/        # Test suite
├── examples/              # Example applications
├── docs/                  # Documentation
│   ├── analysis/         # Analysis documents
│   ├── sprints/          # Sprint documentation
│   └── testing/          # Test reports
└── templates/            # .NET templates
```

## Quick Reference

### Before Committing Checklist

- [ ] Code formatted with CSharpier (`dotnet csharpier .`)
- [ ] All tests pass (`dotnet test`)
- [ ] Documentation updated
- [ ] Commit message follows conventions
- [ ] No orphan files left behind
- [ ] Evidence included (if applicable)
- [ ] Philosophy respected (Developer in Control)

### Common Commands

```bash
# Format code
dotnet csharpier .

# Run all tests
dotnet test

# Run specific category
dotnet test --filter "Category=Unit"

# Build solution
dotnet build

# Restore tools
dotnet tool restore

# Install git hooks
dotnet husky install
```

## Resources

- [Project README](../README.md)
- [Contributing Guide](../CONTRIBUTING.md)
- [Code of Conduct](../CODE_OF_CONDUCT.md)
- [Documentation](../docs/)
- [Philosophy Analysis](../docs/analysis/PHILOSOPHY_COMPLIANCE_ANALYSIS.md)

---

**Remember**: DDAP is about **empowering developers**. Every change should reinforce this principle. If you're not sure, ask: "Does this give developers more control or less?"
