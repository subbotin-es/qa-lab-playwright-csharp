using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class ModalsSection
{
    private readonly IPage _page;

    public  ILocator Modal         => _page.Locator("#test-modal");
    private ILocator OpenButton    => _page.Locator("#open-modal");
    // #modal-confirm has no click handler on the live page; #close-modal (X) is the working dismiss button
    private ILocator CloseButton   => _page.Locator("#close-modal");
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
        await CloseButton.ClickAsync();
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
