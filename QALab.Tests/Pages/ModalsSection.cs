using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class ModalsSection
{
    private readonly IPage _page;

    public  ILocator Modal         => _page.Locator("#test-modal");
    private ILocator OpenButton    => _page.Locator("#open-modal");
    private ILocator ConfirmButton => _page.Locator("#modal-confirm");
    private ILocator CancelButton  => _page.Locator("#modal-cancel");

    public ModalsSection(IPage page)
    {
        _page = page;
    }

    public async Task OpenAsync()
    {
        await OpenButton.ClickAsync();
    }

    public async Task ConfirmAsync()
    {
        await ConfirmButton.ClickAsync();
    }

    public async Task CancelAsync()
    {
        await CancelButton.ClickAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#alerts").ScrollIntoViewIfNeededAsync();
    }
}
