using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class IFrameSection
{
    private readonly IPage _page;

    // C# Playwright uses FrameLocator — same API as TS
    private IFrameLocator Frame => _page.FrameLocator("#iframes iframe");

    public ILocator InnerHeading => Frame.Locator("h1, h2, h3, h4").First;

    public IFrameSection(IPage page)
    {
        _page = page;
    }

    public async Task<string> GetInnerHeadingTextAsync()
    {
        return await InnerHeading.InnerTextAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#iframes").ScrollIntoViewIfNeededAsync();
    }
}
