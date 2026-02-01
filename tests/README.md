# Testing Guidelines

This document describes the testing infrastructure and best practices for the DDAP project.

## Overview

DDAP uses a comprehensive testing approach:

- **Unit tests** for business logic and core functionality
- **Integration tests** for data access and API endpoints
- **E2E tests** for documentation and user workflows

## Running Tests

### Run All Tests

```bash
dotnet test
```

### Run Specific Test Project

```bash
# Core framework tests
dotnet test tests/Ddap.Tests/

# Client library tests
dotnet test tests/Ddap.Client.Core.Tests/
dotnet test tests/Ddap.Client.GraphQL.Tests/
dotnet test tests/Ddap.Client.Grpc.Tests/
dotnet test tests/Ddap.Client.Rest.Tests/

# Integration tests
dotnet test tests/Ddap.IntegrationTests/

# gRPC tests
dotnet test tests/Ddap.Grpc.Tests/
```

### Run with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test Categories

Tests use xUnit traits to categorize:

```bash
# Run specific category
dotnet test --filter "Category=Unit"
dotnet test --filter "Category=Integration"
```

## Documentation Tests

Documentation tests are in `tests/Ddap.Docs.Tests/` and use Playwright for E2E testing:

```bash
dotnet test tests/Ddap.Docs.Tests/
```

Before running, ensure Playwright browsers are installed:

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

## Coverage Requirements

All tests should maintain:
- **80% line coverage** per file
- **80% branch coverage** per file

Coverage is checked automatically in CI:

```bash
./check-coverage.sh
```

## CI/CD Testing

Tests run automatically on:
- Every pull request
- Every push to main
- Scheduled nightly builds

See `.github/workflows/build.yml` for configuration.

## Test Organization

```
tests/
├── Ddap.Tests/                  # Core framework tests (357 tests)
├── Ddap.Client.Core.Tests/      # Client core tests (18 tests)
├── Ddap.Client.GraphQL.Tests/   # GraphQL client tests (18 tests)
├── Ddap.Client.Grpc.Tests/      # gRPC client tests (18 tests)
├── Ddap.Client.Rest.Tests/      # REST client tests (21 tests)
├── Ddap.Grpc.Tests/             # gRPC server tests (40 tests)
├── Ddap.IntegrationTests/       # Integration tests (13 tests)
└── Ddap.Docs.Tests/            # Documentation E2E tests (44 tests)
```

**Total: 529 tests**

## Best Practices

1. **Keep tests fast** - Unit tests should run in milliseconds
2. **Isolate tests** - No shared state between tests
3. **Use descriptive names** - `MethodName_Scenario_ExpectedResult`
4. **Arrange-Act-Assert** - Clear test structure
5. **One assertion per test** - Focus on single behavior
6. **Clean up resources** - Dispose of connections, files, etc.
7. **Mock external dependencies** - Use in-memory databases for unit tests

## Troubleshooting

### Documentation Tests Fail

If documentation tests fail, rebuild the docs:

```bash
docfx build docs/docfx.json
```

### Coverage Too Low

If coverage is below 80%, add more tests:

```bash
# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# View HTML report
open TestResults/*/coverage.cobertura.xml
```

## Related Documentation

- [Contributing Guidelines](../CONTRIBUTING.md) - How to contribute tests
- [Code Coverage](../check-coverage.sh) - Coverage validation script
- [CI/CD](../.github/workflows/build.yml) - Automated testing
