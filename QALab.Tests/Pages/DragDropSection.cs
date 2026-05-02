using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class DragDropSection
{
    private readonly IPage _page;

    public ILocator Draggable => _page.Locator("#drag-1");
    public ILocator DropZone  => _page.Locator("#drag-target");
    public ILocator Slider    => _page.Locator("input[type='range']");

    public DragDropSection(IPage page)
    {
        _page = page;
    }

    // Drags the default draggable element (#drag-1) onto the drop zone (#drag-target)
    public async Task DragItemAsync()
    {
        await Draggable.DragToAsync(DropZone);
    }

    // Overload for custom source/target locators
    public async Task DragItemAsync(ILocator source, ILocator target)
    {
        await source.DragToAsync(target);
    }

    // Playwright supports FillAsync on input[type=range] directly
    public async Task SetSliderValueAsync(string value)
    {
        await Slider.FillAsync(value);
    }

    public async Task<string> GetSliderValueAsync()
    {
        return await Slider.InputValueAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#drag-drop").ScrollIntoViewIfNeededAsync();
    }
}
