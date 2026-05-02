using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class InputsSection
{
    private readonly IPage _page;

    private ILocator TextInput   => _page.Locator("#inputs input[type='text']");
    private ILocator NumberInput => _page.Locator("#inputs input[type='number']");
    private ILocator DateInput   => _page.Locator("#inputs input[type='date']");
    private ILocator SearchInput => _page.Locator("#inputs input[type='search']");
    private ILocator UrlInput    => _page.Locator("#inputs input[type='url']");

    public InputsSection(IPage page)
    {
        _page = page;
    }

    public async Task FillTextAsync(string value)
    {
        await TextInput.FillAsync(value);
    }

    public async Task FillNumberAsync(string value)
    {
        await NumberInput.FillAsync(value);
    }

    public async Task FillDateAsync(string value)
    {
        await DateInput.FillAsync(value);
    }

    public async Task FillSearchAsync(string value)
    {
        await SearchInput.FillAsync(value);
    }

    public async Task FillUrlAsync(string value)
    {
        await UrlInput.FillAsync(value);
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#inputs").ScrollIntoViewIfNeededAsync();
    }
}
