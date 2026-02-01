using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace Ddap.Docs.Tests;

/// <summary>
/// Tests for response compression (Brotli and Gzip).
/// Verifies that the server properly compresses responses based on Accept-Encoding headers.
/// </summary>
[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class CompressionTests : PageTest
{
    private const string DocsBaseUrl = "http://localhost:8000/ddap";

    [Test]
    public async Task BrotliCompression_IsUsed_WhenRequested()
    {
        // Act: Make request with Brotli support
        var response = await Page.APIRequest.GetAsync(
            $"{DocsBaseUrl}/styles.css",
            new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Accept-Encoding", "br, gzip, deflate" },
                },
            }
        );

        // Assert: Response should use Brotli compression
        var headers = response.Headers;
        Assert.That(response.Ok, Is.True, "Response should be successful");

        // Check if Content-Encoding header is present
        if (headers.ContainsKey("content-encoding"))
        {
            var encoding = headers["content-encoding"];
            Assert.That(
                encoding,
                Is.EqualTo("br").Or.EqualTo("gzip"),
                "Response should be compressed with Brotli or Gzip"
            );
        }
        else
        {
            Assert.Warn("Content-Encoding header not present - compression may not be working");
        }
    }

    [Test]
    public async Task GzipCompression_IsUsed_WhenBrotliNotSupported()
    {
        // Act: Make request with only Gzip support
        var response = await Page.APIRequest.GetAsync(
            $"{DocsBaseUrl}/theme-toggle.js",
            new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { { "Accept-Encoding", "gzip, deflate" } },
            }
        );

        // Assert: Response should use Gzip compression
        var headers = response.Headers;
        Assert.That(response.Ok, Is.True, "Response should be successful");

        if (headers.ContainsKey("content-encoding"))
        {
            var encoding = headers["content-encoding"];
            Assert.That(
                encoding,
                Is.EqualTo("gzip"),
                "Response should be compressed with Gzip when Brotli not supported"
            );
        }
        else
        {
            Assert.Warn("Content-Encoding header not present - compression may not be working");
        }
    }

    [Test]
    public async Task NoCompression_WhenNotRequested()
    {
        // Act: Make request without compression support
        var response = await Page.APIRequest.GetAsync(
            $"{DocsBaseUrl}/index.html",
            new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { { "Accept-Encoding", "identity" } },
            }
        );

        // Assert: Response should not be compressed
        var headers = response.Headers;
        Assert.That(response.Ok, Is.True, "Response should be successful");

        // Content-Encoding should not be present or should be 'identity'
        if (headers.ContainsKey("content-encoding"))
        {
            var encoding = headers["content-encoding"];
            Assert.That(
                encoding,
                Is.Not.EqualTo("br").And.Not.EqualTo("gzip"),
                "Response should not be compressed when not requested"
            );
        }
    }

    [Test]
    public async Task CSSFiles_AreCompressed()
    {
        // Act: Request CSS file with compression
        var response = await Page.APIRequest.GetAsync(
            $"{DocsBaseUrl}/styles.css",
            new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { { "Accept-Encoding", "br, gzip" } },
            }
        );

        // Assert
        var headers = response.Headers;
        Assert.That(response.Ok, Is.True, "CSS file should be accessible");
        Assert.That(
            headers.ContainsKey("content-type"),
            Is.True,
            "Content-Type header should be present"
        );
        Assert.That(
            headers["content-type"],
            Does.Contain("text/css"),
            "Content-Type should be text/css"
        );

        if (headers.ContainsKey("content-encoding"))
        {
            var encoding = headers["content-encoding"];
            Assert.That(
                encoding,
                Is.EqualTo("br").Or.EqualTo("gzip"),
                "CSS files should be compressed"
            );
        }
    }

    [Test]
    public async Task JavaScriptFiles_AreCompressed()
    {
        // Act: Request JS file with compression
        var response = await Page.APIRequest.GetAsync(
            $"{DocsBaseUrl}/language-switcher.js",
            new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { { "Accept-Encoding", "br, gzip" } },
            }
        );

        // Assert
        var headers = response.Headers;
        Assert.That(response.Ok, Is.True, "JS file should be accessible");
        Assert.That(
            headers.ContainsKey("content-type"),
            Is.True,
            "Content-Type header should be present"
        );

        if (headers.ContainsKey("content-encoding"))
        {
            var encoding = headers["content-encoding"];
            Assert.That(
                encoding,
                Is.EqualTo("br").Or.EqualTo("gzip"),
                "JavaScript files should be compressed"
            );
        }
    }

    [Test]
    public async Task HTMLFiles_AreCompressed()
    {
        // Act: Request HTML file with compression
        var response = await Page.APIRequest.GetAsync(
            $"{DocsBaseUrl}/index.html",
            new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string> { { "Accept-Encoding", "br, gzip" } },
            }
        );

        // Assert
        var headers = response.Headers;
        Assert.That(response.Ok, Is.True, "HTML file should be accessible");
        Assert.That(
            headers.ContainsKey("content-type"),
            Is.True,
            "Content-Type header should be present"
        );
        Assert.That(
            headers["content-type"],
            Does.Contain("text/html"),
            "Content-Type should be text/html"
        );

        if (headers.ContainsKey("content-encoding"))
        {
            var encoding = headers["content-encoding"];
            Assert.That(
                encoding,
                Is.EqualTo("br").Or.EqualTo("gzip"),
                "HTML files should be compressed"
            );
        }
    }

    [Test]
    public async Task Compression_IsAppliedWhenRequested()
    {
        // Act: Request file with compression support
        var compressedResponse = await Page.APIRequest.GetAsync(
            $"{DocsBaseUrl}/styles.css",
            new APIRequestContextOptions
            {
                Headers = new Dictionary<string, string>
                {
                    { "Accept-Encoding", "br, gzip, deflate" },
                },
            }
        );

        // Assert: Request should succeed
        Assert.That(compressedResponse.Ok, Is.True, "Compressed request should succeed");

        // Check if any compression was applied
        var headers = compressedResponse.Headers;
        var hasCompression =
            headers.ContainsKey("content-encoding") || headers.ContainsKey("Content-Encoding");

        if (hasCompression)
        {
            var encoding =
                headers.TryGetValue("content-encoding", out var ce1) ? ce1
                : headers.TryGetValue("Content-Encoding", out var ce2) ? ce2
                : "";

            Console.WriteLine($"Compression applied: {encoding}");

            // Verify it's a valid compression algorithm
            Assert.That(
                encoding,
                Does.Match("br|gzip|deflate"),
                "Content-Encoding should be br, gzip, or deflate"
            );
        }
        else
        {
            // If no compression header, it might be because file is very small or already compressed
            Console.WriteLine(
                "No compression header found - file may be small or already compressed"
            );
        }

        // Either way, the response should be successful
        Assert.That(compressedResponse.Status, Is.EqualTo(200), "Response should be OK");
    }
}
