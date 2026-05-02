using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("async-buttons")]
public class AsyncButtonsTests : PlaywrightFixture
{
    private AsyncButtonsSection _asyncButtons = null!;

    [SetUp]
    public async Task SetUpAsyncButtonsAsync()
    {
        _asyncButtons = new AsyncButtonsSection(Page);
        await _asyncButtons.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Async button is visible and in idle state before interaction")]
    public async Task AsyncButton_ShouldBeVisibleInitially()
    {
        await Expect(_asyncButtons.AsyncButton).ToBeVisibleAsync();
        await Expect(_asyncButtons.AsyncButton).ToHaveAttributeAsync("data-state", "idle");
    }

    [Test]
    [Category("smoke")]
    [Description("Clicking async button transitions data-state from idle through loading to success")]
    public async Task ClickAsyncButton_ShouldReachSuccessState()
    {
        await _asyncButtons.ClickAsync();
        await Expect(_asyncButtons.AsyncButton).ToHaveAttributeAsync("data-state", "success",
            new LocatorAssertionsToHaveAttributeOptions { Timeout = 10_000 });
    }

    [Test]
    [Category("regression")]
    [Description("Status element is visible and reflects the outcome after the async operation")]
    public async Task AsyncButton_StatusShouldBeUpdated()
    {
        await _asyncButtons.ClickAsync();
        await Expect(_asyncButtons.AsyncButton).ToHaveAttributeAsync("data-state", "success",
            new LocatorAssertionsToHaveAttributeOptions { Timeout = 10_000 });
        await Expect(_asyncButtons.StatusElement).ToBeVisibleAsync();
    }
}
