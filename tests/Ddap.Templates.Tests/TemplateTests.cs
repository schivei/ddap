using System.Diagnostics;
using System.Text.RegularExpressions;
using FluentAssertions;

namespace Ddap.Templates.Tests;

/// <summary>
/// Tests for the DDAP project template
/// </summary>
public class TemplateTests : IDisposable
{
    private readonly string _workingDirectory;
    private readonly string _templatePackagePath;

    public TemplateTests()
    {
        // Use a unique temporary directory for each test run
        _workingDirectory = Path.Combine(
            Path.GetTempPath(),
            $"ddap-template-tests-{Guid.NewGuid():N}"
        );
        Directory.CreateDirectory(_workingDirectory);

        // Find the template package
        var artifactsDir = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "artifacts")
        );
        var templatePackages = Directory.GetFiles(artifactsDir, "Ddap.Templates.*.nupkg");
        _templatePackagePath =
            templatePackages.OrderByDescending(File.GetLastWriteTime).FirstOrDefault()
            ?? throw new FileNotFoundException("Template package not found in artifacts directory");

        // Install the template
        RunCommand("dotnet", $"new install {_templatePackagePath}");
    }

    public void Dispose()
    {
        // Uninstall the template
        try
        {
            RunCommand("dotnet", "new uninstall Ddap.Templates");
        }
        catch
        {
            // Ignore errors during cleanup
        }

        // Clean up working directory
        if (Directory.Exists(_workingDirectory))
        {
            try
            {
                Directory.Delete(_workingDirectory, true);
            }
            catch
            {
                // Ignore errors during cleanup
            }
        }
    }

    [Fact]
    public void Template_Should_BeInstalled()
    {
        // Act
        var output = RunCommand("dotnet", "new list ddap");

        // Assert
        output.Should().Contain("DDAP API");
        output.Should().Contain("ddap-api");
    }

    [Fact]
    public void Template_Should_GenerateBasicProject()
    {
        // Arrange
        var projectName = "TestBasicApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        var output = RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver",
            testDir
        );

        // Assert
        output.Should().Contain("The template \"DDAP API\" was created successfully");

        // Verify project files exist in the subdirectory created by dotnet new
        var projectDir = Path.Combine(testDir, projectName);
        Directory.Exists(projectDir).Should().BeTrue();
        File.Exists(Path.Combine(projectDir, $"{projectName}.csproj")).Should().BeTrue();
        File.Exists(Path.Combine(projectDir, "Program.cs")).Should().BeTrue();
        File.Exists(Path.Combine(projectDir, "appsettings.json")).Should().BeTrue();
        File.Exists(Path.Combine(projectDir, "README.md")).Should().BeTrue();
    }

    [Fact]
    public void Template_Should_IncludeRestPackage_WhenRestEnabled()
    {
        // Arrange
        var projectName = "TestRestApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --rest true",
            testDir
        );

        // Assert
        var projectDir = Path.Combine(testDir, projectName);
        var csprojContent = File.ReadAllText(Path.Combine(projectDir, $"{projectName}.csproj"));
        csprojContent.Should().Contain("Ddap.Rest");

        var programContent = File.ReadAllText(Path.Combine(projectDir, "Program.cs"));
        programContent.Should().Contain("AddRest()");
        programContent.Should().Contain("MapControllers()");
    }

    [Fact]
    public void Template_Should_IncludeGraphQLPackage_WhenGraphQLEnabled()
    {
        // Arrange
        var projectName = "TestGraphQLApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --graphql true",
            testDir
        );

        // Assert
        var projectDir = Path.Combine(testDir, projectName);
        var csprojContent = File.ReadAllText(Path.Combine(projectDir, $"{projectName}.csproj"));
        csprojContent.Should().Contain("Ddap.GraphQL");

        var programContent = File.ReadAllText(Path.Combine(projectDir, "Program.cs"));
        programContent.Should().Contain("AddGraphQL()");
        programContent.Should().Contain("MapGraphQL");
    }

    [Fact]
    public void Template_Should_IncludeMultipleAPIProviders()
    {
        // Arrange
        var projectName = "TestMultiApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --rest true --graphql true --grpc true",
            testDir
        );

        // Assert
        var projectDir = Path.Combine(testDir, projectName);
        var csprojContent = File.ReadAllText(Path.Combine(projectDir, $"{projectName}.csproj"));
        csprojContent.Should().Contain("Ddap.Rest");
        csprojContent.Should().Contain("Ddap.GraphQL");
        csprojContent.Should().Contain("Ddap.Grpc");

        var programContent = File.ReadAllText(Path.Combine(projectDir, "Program.cs"));
        programContent.Should().Contain("AddRest()");
        programContent.Should().Contain("AddGraphQL()");
        programContent.Should().Contain("AddGrpc()");
    }

    [Theory]
    [InlineData("sqlserver", "Ddap.Data.Dapper.SqlServer")]
    [InlineData("mysql", "Ddap.Data.Dapper.MySQL")]
    [InlineData("postgresql", "Ddap.Data.Dapper.PostgreSQL")]
    [InlineData("sqlite", "Ddap.Data.Dapper")]
    public void Template_Should_IncludeCorrectDapperPackage_ForDatabaseType(
        string databaseType,
        string expectedPackage
    )
    {
        // Arrange
        var projectName = $"TestDapper{databaseType}";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type {databaseType}",
            testDir
        );

        // Assert
        var projectDir = Path.Combine(testDir, projectName);
        var csprojContent = File.ReadAllText(Path.Combine(projectDir, $"{projectName}.csproj"));
        csprojContent.Should().Contain(expectedPackage);
    }

    [Fact]
    public void Template_Should_IncludeEntityFrameworkPackages_WhenEFSelected()
    {
        // Arrange
        var projectName = "TestEFApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider entityframework --database-type sqlserver",
            testDir
        );

        // Assert
        var projectDir = Path.Combine(testDir, projectName);
        var csprojContent = File.ReadAllText(Path.Combine(projectDir, $"{projectName}.csproj"));
        csprojContent.Should().Contain("Ddap.Data.EntityFramework");
        csprojContent.Should().Contain("Microsoft.EntityFrameworkCore.SqlServer");
    }

    [Fact]
    public void Template_Should_IncludeAuthPackage_WhenAuthEnabled()
    {
        // Arrange
        var projectName = "TestAuthApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --include-auth true",
            testDir
        );

        // Assert
        var projectDir = Path.Combine(testDir, projectName);
        var csprojContent = File.ReadAllText(Path.Combine(projectDir, $"{projectName}.csproj"));
        csprojContent.Should().Contain("Ddap.Auth");

        var programContent = File.ReadAllText(Path.Combine(projectDir, "Program.cs"));
        programContent.Should().Contain("AddAuthentication");
        programContent.Should().Contain("UseAuthentication()");

        var appsettingsContent = File.ReadAllText(Path.Combine(projectDir, "appsettings.json"));
        appsettingsContent.Should().Contain("Jwt");
    }

    [Fact]
    public void Template_Should_IncludeSubscriptionsPackage_WhenSubscriptionsEnabled()
    {
        // Arrange
        var projectName = "TestSubsApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --include-subscriptions true",
            testDir
        );

        // Assert
        var projectDir = Path.Combine(testDir, projectName);
        var csprojContent = File.ReadAllText(Path.Combine(projectDir, $"{projectName}.csproj"));
        csprojContent.Should().Contain("Ddap.Subscriptions");

        var programContent = File.ReadAllText(Path.Combine(projectDir, "Program.cs"));
        programContent.Should().Contain("AddSubscriptions()");
    }

    [Fact]
    public void Template_Should_IncludeAspireProjects_WhenAspireEnabled()
    {
        // Arrange
        var projectName = "TestAspireApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));

        // Act
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --use-aspire true",
            testDir
        );

        // Assert
        var projectDir = Path.Combine(testDir, projectName);
        Directory.Exists(Path.Combine(projectDir, $"{projectName}.AppHost")).Should().BeTrue();
        Directory
            .Exists(Path.Combine(projectDir, $"{projectName}.ServiceDefaults"))
            .Should()
            .BeTrue();

        File.Exists(
                Path.Combine(projectDir, $"{projectName}.AppHost", $"{projectName}.AppHost.csproj")
            )
            .Should()
            .BeTrue();
        File.Exists(
                Path.Combine(
                    projectDir,
                    $"{projectName}.ServiceDefaults",
                    $"{projectName}.ServiceDefaults.csproj"
                )
            )
            .Should()
            .BeTrue();
    }

    [Fact]
    public void GeneratedProject_Should_RestoreSuccessfully()
    {
        // Arrange
        var projectName = "TestRestoreApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --rest true",
            testDir
        );

        // Act
        var projectDir = Path.Combine(testDir, projectName);
        var output = RunCommand("dotnet", "restore", projectDir);

        // Assert
        output.Should().Contain("Restored");
    }

    [Fact]
    public void GeneratedProject_Should_BuildSuccessfully()
    {
        // Arrange
        var projectName = "TestBuildApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --rest true",
            testDir
        );

        // Act
        var projectDir = Path.Combine(testDir, projectName);
        RunCommand("dotnet", "restore", projectDir);
        var output = RunCommand("dotnet", "build --no-restore", projectDir);

        // Assert
        output.Should().Contain("Build succeeded");
    }

    [Fact]
    public void GeneratedProject_WithAllFeatures_Should_BuildSuccessfully()
    {
        // Arrange
        var projectName = "TestFullApi";
        var testDir = Path.Combine(_workingDirectory, Guid.NewGuid().ToString("N"));
        RunCommand(
            "dotnet",
            $"new ddap-api --name {projectName} --database-provider dapper --database-type sqlserver --rest true --graphql true --grpc true --include-auth true --include-subscriptions true",
            testDir
        );

        // Act
        var projectDir = Path.Combine(testDir, projectName);
        RunCommand("dotnet", "restore", projectDir);
        var output = RunCommand("dotnet", "build --no-restore", projectDir);

        // Assert
        output.Should().Contain("Build succeeded");
    }

    private string RunCommand(string command, string arguments, string? workingDirectory = null)
    {
        workingDirectory ??= _workingDirectory;

        // Create working directory if it doesn't exist
        if (!Directory.Exists(workingDirectory))
        {
            Directory.CreateDirectory(workingDirectory);
        }

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = command,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };

        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0 && !string.IsNullOrEmpty(error))
        {
            throw new Exception(
                $"Command '{command} {arguments}' failed with exit code {process.ExitCode}. Error: {error}"
            );
        }

        return output + error;
    }
}
