// Fixtures/PlaywrightFixture.cs
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace QALab.Tests.Fixtures;

/// <summary>
/// Base class for all QA Lab tests.
/// Inherits from PlaywrightTest which provides IPage, IBrowser, IBrowserContext.
/// Sets base URL from environment variable with fallback.
/// </summary>
[Parallelizable(ParallelScope.Self)]
public class PlaywrightFixture : PageTest
{
    protected string BaseUrl { get; private set; } = string.Empty;

    [SetUp]
    public async Task SetUpAsync()
    {
        BaseUrl = Environment.GetEnvironmentVariable("BASE_URL")
                  ?? "https://subbotin.es";

        // Navigate to QA Lab before each test
        await Page.GotoAsync($"{BaseUrl}/QA-Lab/qa-lab.html");

        // Verify page loaded
        await Expect(Page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("QA Lab"));
    }

    // Override BrowserNewContextOptions to set viewport
    public override Microsoft.Playwright.BrowserNewContextOptions ContextOptions()
    {
        return new Microsoft.Playwright.BrowserNewContextOptions
        {
            ViewportSize = new Microsoft.Playwright.ViewportSize { Width = 1280, Height = 800 },
            BaseURL = Environment.GetEnvironmentVariable("BASE_URL") ?? "https://subbotin.es",
        };
    }
}
