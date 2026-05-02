using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class QALabPage
{
    private readonly IPage _page;

    public QALabPage(IPage page)
    {
        _page = page;
    }

    public async Task GotoAsync(string baseUrl)
    {
        await _page.GotoAsync($"{baseUrl}/QA-Lab/qa-lab.html");
    }

    public async Task ScrollToSectionAsync(string sectionId)
    {
        await _page.Locator($"#{sectionId}").ScrollIntoViewIfNeededAsync();
    }
}
