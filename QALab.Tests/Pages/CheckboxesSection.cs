using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class CheckboxesSection
{
    private readonly IPage _page;

    // Public for test assertions — .Nth(index) access (0-based)
    public ILocator Checkboxes   => _page.Locator("#checkboxes input[type='checkbox']");
    public ILocator RadioButtons => _page.Locator("#checkboxes input[type='radio']");

    public CheckboxesSection(IPage page)
    {
        _page = page;
    }

    public async Task CheckAsync(int index)
    {
        await Checkboxes.Nth(index).CheckAsync();
    }

    public async Task UncheckAsync(int index)
    {
        await Checkboxes.Nth(index).UncheckAsync();
    }

    public async Task<bool> IsCheckedAsync(int index)
    {
        return await Checkboxes.Nth(index).IsCheckedAsync();
    }

    public async Task<bool> IsCheckboxDisabledAsync(int index)
    {
        return await Checkboxes.Nth(index).IsDisabledAsync();
    }

    public async Task SelectRadioAsync(int index)
    {
        await RadioButtons.Nth(index).CheckAsync();
    }

    public async Task<bool> IsRadioCheckedAsync(int index)
    {
        return await RadioButtons.Nth(index).IsCheckedAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#checkboxes").ScrollIntoViewIfNeededAsync();
    }
}
