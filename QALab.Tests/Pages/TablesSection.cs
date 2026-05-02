using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class TablesSection
{
    private readonly IPage _page;

    private ILocator TableRows => _page.Locator("#tables tbody tr");

    public TablesSection(IPage page)
    {
        _page = page;
    }

    public async Task<int> GetRowCountAsync()
    {
        return await TableRows.CountAsync();
    }

    // row and col are 1-based (CSS nth-child convention)
    public async Task<string> GetCellTextAsync(int row, int col)
    {
        return await _page.Locator($"#tables tbody tr:nth-child({row}) td:nth-child({col})").InnerTextAsync();
    }

    public async Task ClickEditAsync(int row)
    {
        await _page.Locator($"#tables tbody tr:nth-child({row})").GetByRole(AriaRole.Button).ClickAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#tables").ScrollIntoViewIfNeededAsync();
    }
}
