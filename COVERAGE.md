# Code Coverage Configuration

This repository is configured with comprehensive code coverage requirements to ensure high code quality.

## Coverage Requirements

- **Per-Class Threshold**: 80% line coverage and 80% branch coverage per class
- **Measurement Level**: Coverage is measured per class/type (standard for .NET projects), not just overall
- **Enforcement**: Coverage checks run on every CI build as warnings (non-blocking)

## Configuration Files

### `.runsettings`
The main coverage configuration file used by `dotnet test`. It configures:
- Coverage data collector (XPlat Code Coverage)
- Exclusion patterns for test assemblies, generated code, and example projects
- Inclusion patterns for main application code

### `coverlet.json`
**Note**: This file is currently not used in the CI pipeline. Coverage configuration is handled entirely through `.runsettings`. This file is provided as a reference for local development if you choose to use Coverlet directly. To use it, you would need to:
- Add `<PackageReference Include="coverlet.msbuild" />` to test projects
- Pass `/p:CollectCoverage=true /p:CoverletOutput=./coverage/` to `dotnet test`

If not using Coverlet directly, the configuration in `.runsettings` is sufficient.

### `check-coverage.sh`
Script that validates per-class coverage thresholds and provides detailed reporting. Note: In .NET, coverage is measured at the class/type level, which typically corresponds to one class per file but may include nested classes.

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

If a class fails the coverage threshold:

1. **Identify the class** - Check the coverage report or CI output
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

- Coverage checks run as warnings (`continue-on-error: true`) to not block builds
- The goal is to track and improve coverage over time, while allowing incremental progress
- Focus on meaningful coverage that tests actual behavior, not just line hits
- Classes with no branches (simple DTOs, options classes) only need line coverage
- When a class fails coverage thresholds, a warning is shown with details
- The CI will still pass even with low coverage, but teams can see what needs improvement
