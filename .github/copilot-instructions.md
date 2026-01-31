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

### Critical Pre-Commit Checklist

**MANDATORY**: Before every commit, ALL of the following must pass:

1. **✅ Code Formatting** - CSharpier must be run
2. **✅ Build Without Warnings** - Zero warnings allowed (TreatWarningsAsErrors=true)
3. **✅ All Tests Pass** - 100% test success rate
4. **✅ Coverage Threshold** - 80% line AND 80% branch coverage per file
5. **✅ Philosophy Compliance** - Follows "Developer in Control" principles
6. **✅ Documentation Valid** - All languages updated and HTML well-formed

### 1. Code Formatting (CSharpier)

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

### 2. Build Without Warnings

**REQUIREMENT**: Code must build with ZERO warnings.

```bash
# Build must pass without any warnings
dotnet build --configuration Release

# This is enforced via TreatWarningsAsErrors=true in Directory.Build.props
```

**Common warnings to avoid:**
- CS0168: Variable declared but never used
- CS0219: Variable assigned but never used  
- CS0414: Field assigned but never used
- CS8600-CS8629: Nullable reference type warnings
- CS1998: Async method lacks 'await' operators

**If you see a warning**: FIX IT. Don't suppress it unless absolutely necessary.

### 3. Test Execution and Coverage

**REQUIREMENT**: All tests must pass AND meet coverage thresholds.

```bash
# Run tests locally
dotnet test --configuration Release

# Check coverage (after running tests)
./check-coverage.sh
```

**Coverage Requirements (STRICT):**
- **80% line coverage** per file (no exceptions)
- **80% branch coverage** per file (no exceptions)
- Files in `.coverage-ignore` are exempt (protobuf, E2E tests)

**When coverage fails:**
1. Add more unit tests to cover the missing lines/branches
2. Use AAA pattern (Arrange, Act, Assert)
3. Test happy path AND edge cases
4. Don't add files to `.coverage-ignore` without approval

### 4. Documentation Validation

**REQUIREMENT**: Documentation must be valid in ALL 7 languages.

```bash
# Validate documentation
./validate-docs.sh
```

**When adding/updating documentation:**
1. Update the English version first
2. Run `generate-locales.js` to update translations
3. Verify all HTML files are well-formed
4. Check for broken internal links
5. Ensure UTF-8 encoding

**Languages that must be maintained:**
- English (en) - base language
- Portuguese Brazil (pt-br)
- Spanish (es)
- French (fr)
- German (de)
- Japanese (ja)
- Chinese (zh)

### 5. Philosophy Compliance Check

**REQUIREMENT**: Code must follow "Developer in Control" principles.

```bash
# Validate philosophy compliance
./validate-philosophy.sh
```

**What this checks:**
- ❌ No forced dependencies (e.g., Pomelo without choice)
- ❌ No hardcoded connection strings
- ❌ No direct database connection instantiation (use DI)
- ❌ No synchronous I/O operations
- ❌ No .Result or .Wait() calls (deadlock risk)
- ❌ No empty catch blocks (hiding errors)

**Before making changes, ask yourself:**
- Does this respect "Developer in Control"?
- Am I forcing a specific implementation?
- Is configuration explicit and overridable?
- Am I using official packages?
- Am I imposing architectural decisions?

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

### Including Evidence (IMPORTANT)

**Evidence Location Rules**:

#### When PR Exists
- ✅ **Include evidence directly in the PR description**
- ✅ Use markdown formatting for screenshots, code samples, test results
- ✅ Evidence becomes part of the permanent PR record
- ✅ No temporary files needed

#### When PR Does NOT Exist Yet
- ✅ **Create a temporary evidence file**: `EVIDENCE_<feature-name>.md`
- ✅ Store all evidence (screenshots, test results, examples) in this file
- ✅ Commit this file with your changes
- ✅ **When creating PR**: Copy evidence content to PR description
- ✅ **After PR is created**: Delete the temporary file in a follow-up commit

**Example Workflow**:

```bash
# 1. Working on feature (no PR yet)
# Create evidence file
cat > EVIDENCE_custom-routes.md << 'EOF'
## Evidence for Custom Routes Feature

### Before
[Screenshot showing limitation]

### After
[Screenshot showing new feature]

### Test Results
```
dotnet test --filter "CustomRoutes"
# All tests passing
```

### Example Usage
```csharp
app.MapDdapRoutes("/api/v2");
```
EOF

# 2. Commit with evidence file
git add .
git commit -m "feat(rest): add custom route prefix support"

# 3. Push and create PR
git push origin feature/custom-routes
# Create PR on GitHub

# 4. Copy evidence from EVIDENCE_custom-routes.md to PR description
# Then delete the evidence file

git rm EVIDENCE_custom-routes.md
git commit -m "chore: remove evidence file after PR creation"
git push
```

**Evidence File Naming Convention**:
- Format: `EVIDENCE_<brief-description>.md`
- Examples:
  - `EVIDENCE_mysql-support.md`
  - `EVIDENCE_graphql-mutations.md`
  - `EVIDENCE_auth-improvements.md`

**What to Include in Evidence**:

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

### Performance Impact (if applicable)
- Before: X ms
- After: Y ms
- Improvement: Z%
```

**When to Skip Evidence**:
- Meta-tasks (documentation updates, refactoring without behavior change)
- Internal tooling changes
- Dependency updates
- Configuration changes

**When Evidence is REQUIRED**:
- New features (user-facing or API changes)
- Bug fixes (show before/after)
- Performance improvements
- UI/UX changes
- Breaking changes

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

# Delete temporary files (including evidence files after PR creation)
rm -f TEMP_*.md WIP_*.md DRAFT_*.md EVIDENCE_*.md
```

### Evidence File Lifecycle

**IMPORTANT**: Evidence files are temporary!

1. **Create**: `EVIDENCE_feature-name.md` when working without PR
2. **Use**: Copy content to PR description when creating PR
3. **Delete**: Remove evidence file after PR is created

```bash
# After copying evidence to PR description
git rm EVIDENCE_*.md
git commit -m "chore: remove evidence file after PR creation"
```

**Never keep evidence files after PR creation** - they should only exist temporarily while work is in progress without a PR.

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
- **Evidence files (delete after PR creation)** ⚠️

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

## Validation Scripts

### Quality Enforcement Scripts

DDAP includes several validation scripts to ensure code quality. These are run automatically in CI/CD but should also be run locally before committing.

#### 1. check-coverage.sh - Coverage Validation

**Purpose**: Ensures every file meets 80% line AND 80% branch coverage.

```bash
# Run after executing tests with coverage
./check-coverage.sh
```

**What it checks:**
- ✅ 80% line coverage per file (strict, no exceptions)
- ✅ 80% branch coverage per file (strict, no exceptions)  
- ✅ Overall coverage statistics
- ❌ Fails build if ANY file is below threshold

**Files exempt** (in `.coverage-ignore`):
- Protobuf generated files
- E2E test files that require browser automation

**When it fails:**
1. Add more unit tests to cover missing lines/branches
2. Test both happy path AND edge cases
3. Use AAA pattern (Arrange, Act, Assert)
4. Don't request exemptions - meet the threshold

#### 2. validate-docs.sh - Documentation Validation

**Purpose**: Ensures documentation is complete and valid in all 7 languages.

```bash
# Run to validate all documentation
./validate-docs.sh
```

**What it checks:**
- ✅ All required pages exist in English (base language)
- ✅ All translations complete for 7 languages (en, pt-br, es, fr, de, ja, zh)
- ✅ HTML structure is well-formed (html, head, body tags)
- ✅ UTF-8 charset declared
- ⚠️ Internal links are not broken (warning only)

**Languages validated:**
- English (en) - base documentation
- Portuguese Brazil (pt-br)
- Spanish (es)
- French (fr)
- German (de)
- Japanese (ja)
- Chinese (zh)

**When it fails:**
1. Complete missing translations
2. Fix HTML structural issues
3. Run `generate-locales.js` to update translations
4. Verify UTF-8 encoding

#### 3. validate-philosophy.sh - Philosophy Compliance

**Purpose**: Ensures code follows "Developer in Control" principles.

```bash
# Run to check philosophy compliance
./validate-philosophy.sh
```

**What it checks:**
- ❌ No forced dependencies (e.g., Pomelo without choice)
- ❌ No hardcoded connection strings
- ❌ No direct database connection instantiation
- ❌ No synchronous I/O operations  
- ❌ No .Result or .Wait() calls (deadlock risk)
- ❌ No empty catch blocks (hiding errors)
- ✅ Dependency injection used properly
- ✅ All I/O operations are async

**When it fails:**
1. Review the specific violations reported
2. Refactor code to follow philosophy principles
3. Use dependency injection instead of direct instantiation
4. Convert synchronous I/O to async/await
5. Remove blocking calls (.Result, .Wait())

**Philosophy principles enforced:**
- **No Forced Dependencies**: Let developers choose implementations
- **Explicit Over Implicit**: Make configuration transparent
- **Async/Await**: All I/O operations must be async
- **Official Packages**: Prefer vendor-supported packages

### Running All Validations Locally

Before pushing code, run all validations:

```bash
# 1. Format code
dotnet csharpier .

# 2. Build without warnings
dotnet build --configuration Release

# 3. Run tests with coverage
dotnet test --configuration Release --settings .runsettings --collect:"XPlat Code Coverage"

# 4. Check coverage thresholds
./check-coverage.sh

# 5. Validate documentation
./validate-docs.sh

# 6. Validate philosophy compliance
./validate-philosophy.sh

# If all pass, commit!
git add .
git commit -m "feat: your feature description"
```

### CI/CD Integration

These scripts are automatically run in GitHub Actions:

- **Build Workflow** (`.github/workflows/build.yml`):
  - ✅ Code formatting check
  - ✅ Build with TreatWarningsAsErrors
  - ✅ Test execution
  - ✅ Coverage validation (`check-coverage.sh`)
  - ✅ Documentation validation (`validate-docs.sh`)

- **Copilot Review Workflow** (`.github/workflows/copilot-review.yml`):
  - ✅ Philosophy compliance check (`validate-philosophy.sh`)
  - ✅ Automated review comment on PRs

**All checks must pass for PR merge.**

## Quick Reference

### Before Committing Checklist

- [ ] ✅ **Code formatted** with CSharpier (`dotnet csharpier .`)
- [ ] ✅ **Build passes** without warnings (`dotnet build --configuration Release`)
- [ ] ✅ **All tests pass** (`dotnet test --configuration Release`)
- [ ] ✅ **Coverage thresholds met** (`./check-coverage.sh` - 80% line & branch per file)
- [ ] ✅ **Documentation validated** (`./validate-docs.sh` - all 7 languages)
- [ ] ✅ **Philosophy compliance** (`./validate-philosophy.sh` - "Developer in Control")
- [ ] ✅ **Commit message** follows Conventional Commits format
- [ ] ✅ **No orphan files** left behind (TEMP_*, WIP_*, analysis docs moved to docs/)
- [ ] ✅ **Evidence included** (if feature change: screenshots, tests, examples)
- [ ] ✅ **Documentation updated** (if user-facing change)

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
