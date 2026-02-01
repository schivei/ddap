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

## Pre-Commit Setup (MANDATORY)

### ⚠️ CRITICAL RULE: NEVER USE --no-verify ⚠️

**HIGHEST PRIORITY MANDATORY RULE:**

```bash
# ❌ NEVER DO THIS - ABSOLUTELY FORBIDDEN
git commit --no-verify -m "anything"

# ❌ NEVER BYPASS HUSKY HOOKS
git commit -n -m "anything"

# ✅ ALWAYS commit normally - let Husky do its job
git commit -m "your message"
```

**Why this is forbidden:**
- Bypasses ALL quality checks (formatting, tests, validation)
- Can introduce broken code into the repository
- Defeats the entire purpose of automated quality gates
- Creates technical debt
- Can break CI/CD pipelines
- Violates DDAP code quality standards

**What to do instead:**
1. Fix the failing tests/checks
2. Ensure code is correct
3. Commit normally (Husky will validate)
4. If tests fail, fix them - don't bypass

**Exception: NONE**
There are NO valid exceptions to this rule. Ever.

### Initial Setup (Run Once)

**ALWAYS** run these commands when starting work on the project:

```bash
# 1. Restore .NET tools (CSharpier, Husky)
dotnet tool restore

# 2. Install Husky git hooks
dotnet husky install

# 3. Verify CSharpier is available
dotnet csharpier --version
```

### What Husky Does Automatically

Once installed, Husky runs on every `git commit`:

1. ✅ **Restores .NET tools** automatically
2. ✅ **Formats code with CSharpier** automatically
3. ✅ **Stages formatted files** automatically
4. ✅ **Restores project dependencies** automatically
5. ✅ **Runs all tests** automatically
6. ✅ **Validates static files** automatically
7. ✅ **Aborts commit if tests fail** automatically

**You don't need to do anything manually!** Just commit normally:

```bash
git add .
git commit -m "feat: your feature"
# Husky automatically formats, tests, validates
# Commit succeeds only if everything passes
```

If the commit is rejected, it means something is wrong. Fix it, don't bypass it.

## Quick Reference

### Before Committing Checklist (Now Mostly Automated)

The Husky pre-commit hook now handles most of these automatically:

- [x] ✅ **Code formatted** with CSharpier - **AUTOMATED BY HUSKY**
- [x] ✅ **Build passes** without warnings - **AUTOMATED BY HUSKY** (via tests)
- [x] ✅ **All tests pass** - **AUTOMATED BY HUSKY**
- [x] ✅ **Static files validated** - **AUTOMATED BY HUSKY**
- [ ] ✅ **Coverage thresholds met** (`./check-coverage.sh` - 80% line & branch per file) - MANUAL
- [ ] ✅ **Documentation validated** (`./validate-docs.sh` - all 7 languages) - MANUAL
- [ ] ✅ **Philosophy compliance** (`./validate-philosophy.sh` - "Developer in Control") - MANUAL
- [ ] ✅ **Commit message** follows Conventional Commits format - MANUAL
- [ ] ✅ **No orphan files** left behind (TEMP_*, WIP_*, analysis docs moved to docs/) - MANUAL
- [ ] ✅ **Evidence included** (if feature change: screenshots, tests, examples) - MANUAL
- [ ] ✅ **Documentation updated** (if user-facing change) - MANUAL

**4 of 10 checks are fully automated!**

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

# Validate static files
./validate-static-files.sh
```

## Static File Validation

### Overview

All static files (Markdown, JSON, YAML, XML) must be valid and well-formatted. The `validate-static-files.sh` script checks these files automatically in the pre-commit hook.

### Why Validate Static Files?

- ✅ Prevent syntax errors in configuration files
- ✅ Maintain consistent documentation formatting
- ✅ Catch issues before they reach CI/CD
- ✅ Professional code quality standards
- ✅ Better readability and maintainability

### Validated File Types

1. **Markdown** (.md) - Documentation files
2. **JSON** (.json) - Configuration and package files
3. **YAML** (.yml, .yaml) - GitHub Actions, Docker, configs
4. **XML** (.xml, .csproj, .props, .targets) - Project files

### Markdown Best Practices

**Standards**:
- No trailing whitespace
- Maximum line length: 120 characters (warning)
- Consistent line endings (LF)
- No multiple consecutive blank lines
- Proper heading hierarchy
- Code blocks properly fenced

**Good Example**:
```markdown
# Main Heading

Brief introduction paragraph that doesn't exceed 120 characters.

## Subheading

- Bullet point one
- Bullet point two

```csharp
// Code block with proper fencing
public class Example { }
```

## Another Section

Content here.
```

**Bad Example**:
```markdown
#Main Heading      <- trailing spaces, no space after #

This is a very long line that exceeds 120 characters and should be broken into multiple lines for better readability and maintenance.


<- multiple blank lines

## Subheading
- Bullet one
  -  Inconsistent indentation
```

### JSON Best Practices

**Standards**:
- Valid JSON syntax (use linter/validator)
- 2-space indentation
- No trailing commas
- Double quotes only (not single quotes)
- Proper escaping
- UTF-8 encoding

**Good Example**:
```json
{
  "name": "ddap",
  "version": "1.0.0",
  "dependencies": {
    "package1": "1.0.0",
    "package2": "2.0.0"
  },
  "scripts": {
    "build": "dotnet build"
  }
}
```

**Bad Example**:
```json
{
  'name': 'ddap',  // ❌ Single quotes not allowed
  "version": "1.0.0",
  "dependencies": {
    "package1": "1.0.0",  // ❌ Trailing comma
  },
  "scripts": {
    "build": "dotnet build",  // ❌ Trailing comma
  },  // ❌ Trailing comma
}
```

### YAML Best Practices

**Standards**:
- Valid YAML syntax
- 2-space indentation (no tabs!)
- Consistent quoting
- Proper list formatting
- No trailing whitespace

**Good Example**:
```yaml
name: Build

on:
  push:
    branches:
      - main
  pull_request:

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build
        run: dotnet build
```

**Bad Example**:
```yaml
name: Build
on:
  push:
→ branches:  # ❌ Tab character instead of spaces
      - main
 pull_request:  # ❌ Inconsistent indentation

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3  # ❌ Inconsistent indentation
      - name: Build
        run: dotnet build      # ❌ Trailing spaces
```

### XML Best Practices

**Standards**:
- Well-formed XML
- Proper tag closure
- Valid structure
- 2-space indentation
- Proper encoding declaration

**Good Example**:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Package" Version="1.0.*" />
  </ItemGroup>
</Project>
```

**Bad Example**:
```xml
<?xml version="1.0" encoding="utf-8"?>
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable  # ❌ Missing closing tag
  </PropertyGroup>
  
  <ItemGroup>
    <PackageReference Include="Package" Version="1.0.*">  # ❌ Self-closing tag not closed
  </ItemGroup>
# ❌ Missing closing Project tag
```

### Running Static File Validation

**Automatic (Pre-Commit Hook)**:
```bash
git add .
git commit -m "Your message"
# Automatically runs:
# 1. CSharpier formatting
# 2. Unit tests
# 3. Static file validation ✅
```

**Manual Validation**:
```bash
# Validate all static files
./validate-static-files.sh

# Output shows:
# ✅ Passed files
# ⚠️  Files with warnings (non-blocking)
# ❌ Files with errors (blocking)
```

### Common Issues and Fixes

#### Markdown Issues

**Issue 1: Trailing Whitespace**
```bash
# Find files with trailing whitespace
grep -r ' $' *.md

# Fix: Remove trailing spaces (most editors can do this)
# VS Code: Search for " $" with regex enabled, replace with ""
```

**Issue 2: Long Lines**
```markdown
<!-- Break long lines into multiple lines -->
This is a very long line that should be broken.

<!-- Becomes: -->
This is a very long line
that should be broken.
```

#### JSON Issues

**Issue 1: Trailing Commas**
```json
// Remove trailing comma before closing brace/bracket
{
  "key": "value",  // ❌ Remove comma
}

// Correct:
{
  "key": "value"
}
```

**Issue 2: Single Quotes**
```json
// Replace single quotes with double quotes
{ 'key': 'value' }  // ❌

// Correct:
{ "key": "value" }  // ✅
```

#### YAML Issues

**Issue 1: Tabs Instead of Spaces**
```bash
# Find tabs in YAML
grep -P '\t' *.yml

# Fix: Replace tabs with 2 spaces
```

**Issue 2: Inconsistent Indentation**
```yaml
# Use consistent 2-space indentation
list:
  - item1
  - item2  # ✅ Consistent

# Not:
list:
  - item1
   - item2  # ❌ Wrong indentation
```

### Integration with CI/CD

The static file validation is integrated into:

1. **Pre-Commit Hook** (.husky/pre-commit)
   - Runs automatically on `git commit`
   - Blocks commit if validation fails
   - Fast feedback loop

2. **CI/CD Pipeline** (.github/workflows/build.yml)
   - Runs on every pull request
   - Ensures all files pass validation
   - Maintains code quality standards

### No npm Dependencies

The validation script uses only:
- ✅ Python 3 (standard on most systems)
- ✅ Bash (Unix/Linux/macOS standard)
- ✅ Git (required for repository)

**No npm required!** This keeps the toolchain simple and dependencies minimal.

### Quick Checklist for Static Files

Before committing:
- [ ] Markdown: No trailing spaces, reasonable line length
- [ ] JSON: Valid syntax, proper formatting
- [ ] YAML: Valid syntax, spaces (not tabs), consistent indentation
- [ ] XML: Well-formed, proper structure
- [ ] Run `./validate-static-files.sh` to verify

## Resources

- [Project README](../README.md)
- [Contributing Guide](../CONTRIBUTING.md)
- [Code of Conduct](../CODE_OF_CONDUCT.md)
- [Documentation](../docs/)
- [Philosophy Analysis](../docs/analysis/PHILOSOPHY_COMPLIANCE_ANALYSIS.md)

---

**Remember**: DDAP is about **empowering developers**. Every change should reinforce this principle. If you're not sure, ask: "Does this give developers more control or less?"
