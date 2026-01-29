# Code Coverage Configuration

This repository is configured with comprehensive code coverage requirements to ensure high code quality.

## Coverage Requirements

- **Per-File Threshold**: 80% line coverage and 80% branch coverage per file
- **Measurement Level**: Coverage is measured per file, not just overall
- **Enforcement**: Coverage checks run on every CI build

## Configuration Files

### `.runsettings`
The main coverage configuration file used by `dotnet test`. It configures:
- Coverage data collector (XPlat Code Coverage)
- Exclusion patterns for test assemblies, generated code, and example projects
- Inclusion patterns for main application code

### `coverlet.json`
Additional configuration for Coverlet coverage tool, specifying:
- File patterns to exclude from coverage
- Attributes to exclude (Obsolete, GeneratedCode, etc.)
- Coverage thresholds

### `check-coverage.sh`
Script that validates per-file coverage thresholds and provides detailed reporting.

## Excluded from Coverage

The following modules and files are excluded from coverage requirements:

### Modules
- **Data Providers** (`Ddap.Data.*`, `Ddap.Data.Dapper.*`) - Require actual database connections for full coverage
- **Auth Module** (`Ddap.Auth`) - Requires complex authentication setup
- **CodeGen Module** (`Ddap.CodeGen`) - Requires specific generation conditions
- **Memory Module** (`Ddap.Memory`) - Simple in-memory implementation
- **Subscriptions Module** (`Ddap.Subscriptions`) - Requires messaging infrastructure
- **Test Projects** (`*.Tests.*`) - Test code is not covered
- **Example Projects** (`*.Example.*`) - Examples are not covered

### Files
- Generated files (`*.g.cs`, `**/Generated/**`)
- Assembly info files
- Internal configuration classes
- Auto-reload options classes

## Included in Coverage

Core modules that must meet coverage thresholds:
- **Ddap.Core** - Core functionality and abstractions
- **Ddap.Rest** - REST API provider
- **Ddap.GraphQL** - GraphQL API provider
- **Ddap.Grpc** - gRPC API provider
- **Ddap.Aspire** - .NET Aspire integration

## Running Coverage Locally

### Run tests with coverage
```bash
dotnet test --settings .runsettings --collect:"XPlat Code Coverage" --results-directory ./coverage
```

### Generate coverage report
```bash
reportgenerator \
  -reports:"coverage/**/coverage.cobertura.xml" \
  -targetdir:"coverage/report" \
  -reporttypes:"Html;TextSummary;JsonSummary" \
  -verbosity:Info \
  -classfilters:"-*.Tests.*;-*.Example.*;-Ddap.Data.*;-Ddap.Auth.*;-Ddap.CodeGen.*;-Ddap.Memory.*;-Ddap.Subscriptions.*" \
  -filefilters:"-*.g.cs;-*Generated*"
```

### Check per-file thresholds
```bash
./check-coverage.sh
```

### View HTML report
Open `coverage/report/index.html` in your browser.

## CI/CD Integration

The GitHub Actions workflow (`.github/workflows/build.yml`) automatically:
1. Runs tests with coverage collection
2. Generates coverage reports with filters
3. Checks per-file coverage thresholds
4. Posts coverage summary to pull requests
5. Uploads coverage data to Codecov

## Improving Coverage

If a file fails the coverage threshold:

1. **Identify the file** - Check the coverage report or CI output
2. **Review uncovered lines** - Open the HTML report to see which lines need coverage
3. **Write tests** - Add unit tests that exercise the uncovered code paths
4. **Verify** - Run coverage locally to confirm improvements
5. **Commit** - Push your changes and verify in CI

## Coverage Report Formats

The pipeline generates multiple report formats:
- **HTML** - Interactive web report at `coverage/report/index.html`
- **JSON** - Machine-readable summary at `coverage/report/Summary.json`
- **Markdown** - GitHub-formatted summary for PR comments
- **Cobertura** - XML format for Codecov integration

## Notes

- Coverage checks are set to `continue-on-error: true` to not block builds, but warnings are shown
- The goal is to improve coverage over time, not to block all changes
- Focus on meaningful coverage that tests actual behavior, not just line hits
- Files with no branches (simple DTOs, options classes) only need line coverage
