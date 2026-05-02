using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class ButtonsSection
{
    private readonly IPage _page;

    private ILocator PrimaryButton  => _page.Locator("#buttons").GetByRole(AriaRole.Button, new() { Name = "Primary Button",  Exact = true });
    private ILocator DangerButton   => _page.Locator("#buttons").GetByRole(AriaRole.Button, new() { Name = "Danger Button",   Exact = true });
    public  ILocator DisabledButton => _page.Locator("#buttons").GetByRole(AriaRole.Button, new() { Name = "Disabled Button", Exact = true });

    public ButtonsSection(IPage page)
    {
        _page = page;
    }

    public async Task ClickPrimaryAsync()
    {
        await PrimaryButton.ClickAsync();
    }

    public async Task ClickDangerAsync()
    {
        await DangerButton.ClickAsync();
    }

    public async Task<bool> IsPrimaryButtonVisibleAsync()
    {
        return await PrimaryButton.IsVisibleAsync();
    }

    public async Task<bool> IsDisabledButtonDisabledAsync()
    {
        return await DisabledButton.IsDisabledAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#buttons").ScrollIntoViewIfNeededAsync();
    }
}
