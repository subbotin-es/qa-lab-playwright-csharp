using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class AsyncButtonsSection
{
    private readonly IPage _page;

    // Submit Order button — data-state: idle → loading → success
    public ILocator AsyncButton   => _page.Locator("#btn-async-success");
    public ILocator StatusElement => _page.Locator("#async-status");

    public AsyncButtonsSection(IPage page)
    {
        _page = page;
    }

    public async Task ClickAsync()
    {
        await AsyncButton.ClickAsync();
    }

    // Waits until data-state attribute equals expectedState, then returns button text.
    // Uses page.WaitForFunctionAsync — no Task.Delay, no Thread.Sleep.
    public async Task<string> WaitForStateAsync(string expectedState)
    {
        await _page.WaitForFunctionAsync(
            $"document.querySelector('#btn-async-success').getAttribute('data-state') === '{expectedState}'"
        );
        return await AsyncButton.InnerTextAsync();
    }

    public async Task<string> GetStatusTextAsync()
    {
        return await StatusElement.InnerTextAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#async-buttons").ScrollIntoViewIfNeededAsync();
    }
}
