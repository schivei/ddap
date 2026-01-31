using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Find the docs/_site directory
var docsPath = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "_site")
);

Console.WriteLine($"Documentation path: {docsPath}");

if (!Directory.Exists(docsPath))
{
    Console.Error.WriteLine($"ERROR: Documentation directory not found at {docsPath}");
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

Console.WriteLine("Starting server on http://localhost:8000");
app.Run("http://localhost:8000");
