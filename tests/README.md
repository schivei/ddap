# Template Tests

Automated tests for the `ddap-api` template to ensure it generates valid, buildable projects across all supported configurations.

## Overview

The DDAP template tests validate that:
- Templates generate correctly with all parameter combinations
- Generated projects restore dependencies successfully
- Generated projects build without errors or warnings
- All database providers work correctly (SQL Server, MySQL, PostgreSQL, SQLite)
- All data access providers work correctly (Dapper, Entity Framework)
- All API types work correctly (REST, GraphQL, gRPC)
- Optional features work correctly (Auth, Subscriptions, Aspire)

## Running Tests

### Automated Tests (Unit Tests)

The main test suite is in `Ddap.Templates.Tests` and runs with xUnit:

```bash
# Run all template tests
dotnet test tests/Ddap.Templates.Tests/

# Run from solution root
dotnet test --filter "FullyQualifiedName~Ddap.Templates.Tests"
```

These tests:
- Are included in the test suite at `tests/Ddap.Templates.Tests/`
- Run with `dotnet test`
- Use xUnit and FluentAssertions
- Cover 15+ scenarios for template generation, package inclusion, and build verification

### Validation Scripts (Quick Smoke Tests)

For quick validation during development, use the platform-specific scripts:

#### Linux/Mac

```bash
./tests/validate-template.sh
# or
./tests/validate-template
```

#### Windows PowerShell

```powershell
.\tests\validate-template.ps1
# or
.\tests\validate-template
```

#### Windows cmd

```cmd
tests\validate-template.cmd
```

## Cross-Platform Support

The validation scripts work on:
- ✅ Linux (bash)
- ✅ macOS (bash)
- ✅ Windows PowerShell (pwsh/powershell)
- ✅ Windows Command Prompt (cmd)

The universal wrapper script (`validate-template`) automatically detects your platform and runs the appropriate implementation.

## Test Coverage

### Unit Tests (15+ scenarios)
- Template installation and listing
- Basic project generation
- REST API package inclusion
- GraphQL API package inclusion
- Multiple API providers
- Database-specific Dapper packages (SQL Server, MySQL, PostgreSQL, SQLite)
- Entity Framework packages
- Authentication feature
- Subscriptions feature
- Aspire orchestration
- Project restore and build

### Validation Scripts (23 scenarios)
1. **Database Provider Tests (8 scenarios)**
   - SQL Server + Dapper
   - SQL Server + Entity Framework
   - MySQL + Dapper
   - MySQL + Entity Framework
   - PostgreSQL + Dapper
   - PostgreSQL + Entity Framework
   - SQLite + Dapper
   - SQLite + Entity Framework

2. **API Provider Tests (7 scenarios)**
   - REST only
   - GraphQL only
   - gRPC only
   - REST + GraphQL
   - REST + gRPC
   - GraphQL + gRPC
   - All APIs (REST + GraphQL + gRPC)

3. **Feature Tests (4 scenarios)**
   - With Authentication
   - With Subscriptions
   - With Aspire
   - All Features

4. **Complex Combinations (4 scenarios)**
   - Minimal configuration
   - Maximum configuration (all features enabled)

## CI/CD Integration

Template tests are integrated into the CI/CD pipeline:

### GitHub Actions

The tests run automatically on:
- Pull requests to `main`
- Pushes to `main`
- Manual workflow dispatch

Multi-platform testing matrix:
- Ubuntu (latest)
- Windows (latest)
- macOS (latest)

See `.github/workflows/build.yml` for the full CI configuration.

## Adding New Tests

### Adding Unit Tests

Add new tests to `tests/Ddap.Templates.Tests/TemplateTests.cs`:

```csharp
[Fact]
public void Template_Should_DoSomething()
{
    // Arrange
    var projectName = "TestProject";
    var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

    // Act
    RunCommand(
        "dotnet",
        $"new ddap-api --name {projectName} --your-param value",
        testDir
    );

    // Assert
    var projectDir = Path.Combine(testDir, projectName);
    File.Exists(Path.Combine(projectDir, "expected-file.cs")).Should().BeTrue();
}
```

### Adding Validation Script Tests

To add new validation scenarios, update all three script variants:
1. `validate-template.sh` (Linux/Mac)
2. `validate-template.ps1` (Windows PowerShell)
3. `validate-template.cmd` (Windows cmd)

Keep the test logic synchronized across all platforms.

#### Example: Adding a new test to bash script

```bash
test_template "MyNewTest" "dapper" "sqlserver" "true" "false" "false" "true"
```

#### Example: Adding a new test to PowerShell script

```powershell
Test-Template -Name "MyNewTest" -DatabaseProvider "dapper" -DatabaseType "sqlserver" -Rest "true" -GraphQL "false" -Grpc "false" -IncludeAuth "true"
```

#### Example: Adding a new test to batch script

```batch
call :test_template "MyNewTest" "dapper" "sqlserver" "true" "false" "false" "true"
```

## Template Parameters

The `ddap-api` template supports the following parameters:

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `--database-provider` | choice | `dapper` | ORM/data access provider (`dapper`, `entityframework`) |
| `--database-type` | choice | `sqlserver` | Target database system (`sqlserver`, `mysql`, `postgresql`, `sqlite`) |
| `--rest` | bool | `true` | Enable REST API |
| `--graphql` | bool | `false` | Enable GraphQL |
| `--grpc` | bool | `false` | Enable gRPC |
| `--include-auth` | bool | `false` | Include JWT authentication and authorization |
| `--include-subscriptions` | bool | `false` | Include real-time subscriptions support |
| `--use-aspire` | bool | `false` | Include .NET Aspire orchestration and observability |
| `--connection-string` | string | (empty) | Database connection string (optional) |

## Test Artifacts

Tests create temporary directories for generated projects:
- **Unit tests**: `%TEMP%/ddap-template-tests-{guid}`
- **Validation scripts**: `%TEMP%/ddap-tests-{random}` (Windows) or `/tmp/ddap-tests-{random}` (Unix)

These are automatically cleaned up after test completion.

## Troubleshooting

### Template installation fails

If you get errors about the template already being installed:

```bash
# Uninstall first
dotnet new uninstall Ddap.Templates

# Then reinstall
dotnet new install templates/ddap-api
```

### Generated project fails to build

Check the test output for specific build errors. Common issues:
- Missing package dependencies (run `dotnet restore`)
- Incompatible package versions
- Syntax errors in generated code

### Permission denied errors (Unix)

Make sure scripts are executable:

```bash
chmod +x tests/validate-template.sh
chmod +x tests/validate-template
```

## Development Workflow

When working on the template:

1. **Make changes** to the template files in `templates/ddap-api/`
2. **Run validation** to ensure changes work:
   ```bash
   ./tests/validate-template
   ```
3. **Run full test suite**:
   ```bash
   dotnet test tests/Ddap.Templates.Tests/
   ```
4. **Commit changes** only after all tests pass

## Performance

- **Unit tests**: ~2-5 minutes (runs all 15+ tests)
- **Validation scripts**: ~5-10 minutes (runs 23 scenarios with build verification)
- **CI/CD full matrix**: ~15-30 minutes (runs on 3 platforms)

Tests can be slow because each scenario:
1. Generates a new project
2. Restores NuGet packages
3. Builds the project
4. Cleans up artifacts

This is intentional to ensure comprehensive validation.

## Contributing

When contributing template changes:

1. ✅ **Add tests** for new features or parameters
2. ✅ **Update all scripts** if adding validation scenarios
3. ✅ **Test locally** before pushing
4. ✅ **Document** new parameters in this README
5. ✅ **Follow** the DDAP "Developer in Control" philosophy

## Related Documentation

- [Template Configuration](../templates/ddap-api/.template.config/template.json)
- [Contributing Guide](../CONTRIBUTING.md)
- [DDAP Philosophy](../.github/copilot-instructions.md)
- [Sprint 4 Instructions](../docs/sprints/SPRINT4_PR_INSTRUCTIONS.md)

---

**Last Updated**: 2026-01-31  
**Sprint**: Sprint 4 - Template Testing  
**Status**: ✅ Complete
