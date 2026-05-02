using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class DropdownsSection
{
    private readonly IPage _page;

    public ILocator SingleDropdown => _page.Locator("#dropdowns select:not([multiple])");
    public ILocator MultiDropdown  => _page.Locator("#dropdowns select[multiple]");

    public DropdownsSection(IPage page)
    {
        _page = page;
    }

    public async Task SelectByValueAsync(string value)
    {
        await SingleDropdown.SelectOptionAsync(new SelectOptionValue { Value = value });
    }

    public async Task<string> GetSelectedValueAsync()
    {
        return await SingleDropdown.InputValueAsync();
    }

    public async Task SelectMultipleByValueAsync(params string[] values)
    {
        await MultiDropdown.SelectOptionAsync(values);
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#dropdowns").ScrollIntoViewIfNeededAsync();
    }
}
