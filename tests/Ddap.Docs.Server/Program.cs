using System.Diagnostics;
using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add response compression services
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[]
        {
            "text/html",
            "text/css",
            "application/javascript",
            "text/javascript",
            "application/json",
            "text/xml",
            "application/xml",
            "image/svg+xml",
        }
    );
});

// Configure compression levels
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

var app = builder.Build();

// Enable response compression (must be before static files)
app.UseResponseCompression();

// Find the repository root and docs directories
var repoRoot = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")
);
var docsPath = Path.Combine(repoRoot, "docs", "_site");
var docsSourcePath = Path.Combine(repoRoot, "docs");

Console.WriteLine($"Repository root: {repoRoot}");
Console.WriteLine($"Documentation source: {docsSourcePath}");
Console.WriteLine($"Documentation output: {docsPath}");

// Build documentation if it doesn't exist
if (!Directory.Exists(docsPath))
{
    Console.WriteLine("📝 Documentation not found. Building with DocFX...");

    try
    {
        // Run docfx build
        var docfxPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".dotnet",
            "tools",
            "docfx"
        );

        if (!File.Exists(docfxPath))
        {
            Console.WriteLine("⚠️  DocFX not found. Installing...");
            var installProcess = Process.Start(
                new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "tool install -g docfx",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                }
            );
            installProcess?.WaitForExit();
            Console.WriteLine("✅ DocFX installed");
        }

        // Build documentation
        var docfxConfigPath = Path.Combine(docsSourcePath, "docfx.json");
        var buildProcess = Process.Start(
            new ProcessStartInfo
            {
                FileName = docfxPath,
                Arguments = $"build \"{docfxConfigPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = repoRoot,
            }
        );

        if (buildProcess != null)
        {
            buildProcess.WaitForExit();
            if (buildProcess.ExitCode == 0)
            {
                Console.WriteLine("✅ DocFX build completed");
            }
            else
            {
                Console.WriteLine($"⚠️  DocFX build exited with code {buildProcess.ExitCode}");
            }
        }

        // Copy custom HTML/JS/CSS files
        Console.WriteLine("📋 Copying custom HTML/JS/CSS files...");
        foreach (var ext in new[] { "*.html", "*.js", "*.css" })
        {
            foreach (var file in Directory.GetFiles(docsSourcePath, ext))
            {
                var fileName = Path.GetFileName(file);
                var destFile = Path.Combine(docsPath, fileName);
                File.Copy(file, destFile, overwrite: true);
            }
        }
        Console.WriteLine("✅ Custom files copied");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"❌ Error building documentation: {ex.Message}");
        Console.Error.WriteLine(
            "Please run 'docfx build docs/docfx.json' manually from the repository root."
        );
        Environment.Exit(1);
    }
}
else
{
    Console.WriteLine("✅ Documentation already exists");
}

if (!Directory.Exists(docsPath))
{
    Console.Error.WriteLine($"❌ Documentation directory still not found at {docsPath}");
    Environment.Exit(1);
}

// Map /ddap to serve static files
app.MapWhen(
    context => context.Request.Path.StartsWithSegments("/ddap"),
    appBuilder =>
    {
        // Fallback to index.html for directory requests
        appBuilder.Use(
            async (context, next) =>
            {
                var path = context.Request.Path.Value ?? "";
                if (path == "/ddap" || path == "/ddap/")
                {
                    context.Request.Path = "/ddap/index.html";
                }
                await next();
            }
        );

        appBuilder.UseStaticFiles(
            new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(docsPath),
                RequestPath = "/ddap",
            }
        );

        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync($"Not Found: {context.Request.Path}");
        });
    }
);

app.MapGet("/", () => Results.Redirect("/ddap/"));

// Install Playwright browsers if needed
Console.WriteLine("🎭 Checking Playwright browsers...");
try
{
    var testProjectPath = Path.Combine(repoRoot, "tests", "Ddap.Docs.Tests");
    var playwrightScript = Path.Combine(
        testProjectPath,
        "bin",
        "Release",
        "net10.0",
        "playwright.ps1"
    );

    // Check if playwright.ps1 exists
    if (File.Exists(playwrightScript))
    {
        // Try to install browsers (chromium only for headless tests)
        var playwrightProcess = Process.Start(
            new ProcessStartInfo
            {
                FileName = "pwsh",
                Arguments = $"\"{playwrightScript}\" install chromium --with-deps",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            }
        );

        if (playwrightProcess != null)
        {
            var output = playwrightProcess.StandardOutput.ReadToEnd();
            var error = playwrightProcess.StandardError.ReadToEnd();
            playwrightProcess.WaitForExit();

            if (playwrightProcess.ExitCode == 0)
            {
                Console.WriteLine("✅ Playwright browsers ready");
            }
            else
            {
                Console.WriteLine(
                    $"⚠️  Playwright browser installation exited with code {playwrightProcess.ExitCode}"
                );
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Console.WriteLine($"   Error: {error}");
                }
            }
        }
    }
    else
    {
        Console.WriteLine(
            "ℹ️  Playwright script not found. Browsers will be installed on first test run."
        );
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️  Could not install Playwright browsers: {ex.Message}");
    Console.WriteLine("   Tests will attempt to install browsers on first run.");
}

Console.WriteLine("🚀 Starting server on http://localhost:8000");
Console.WriteLine($"📚 Serving documentation from: {docsPath}");
Console.WriteLine("Press Ctrl+C to stop");
app.Run("http://localhost:8000");
