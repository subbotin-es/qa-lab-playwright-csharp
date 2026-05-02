using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class DynamicVisibilitySection
{
    private readonly IPage _page;

    private ILocator ToggleCheckbox => _page.Locator("#dynamic-visibility input[type='checkbox']");
    public  ILocator SecretPanel    => _page.Locator("#dynamic-visibility").GetByText("Secret panel revealed");

    public DynamicVisibilitySection(IPage page)
    {
        _page = page;
    }

    public async Task ToggleAsync()
    {
        await ToggleCheckbox.ClickAsync();
    }

    public async Task<bool> IsPanelVisibleAsync()
    {
        return await SecretPanel.IsVisibleAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#dynamic-visibility").ScrollIntoViewIfNeededAsync();
    }
}
