using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("drag-drop")]
public class DragDropTests : PlaywrightFixture
{
    private DragDropSection _dragDrop = null!;

    [SetUp]
    public async Task SetUpDragDropAsync()
    {
        _dragDrop = new DragDropSection(Page);
        await _dragDrop.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Draggable element and drop zone are visible before interaction")]
    public async Task DragElements_ShouldBeVisibleInitially()
    {
        await Expect(_dragDrop.Draggable).ToBeVisibleAsync();
        await Expect(_dragDrop.DropZone).ToBeVisibleAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("DragAndDropAsync completes without error and drop zone remains accessible")]
    public async Task DragItem_ShouldComplete()
    {
        await _dragDrop.DragItemAsync();
        // Drop zone must remain visible after the operation
        await Expect(_dragDrop.DropZone).ToBeVisibleAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Custom source/target overload executes drag between specified locators")]
    public async Task DragItem_WithExplicitSourceTarget_ShouldComplete()
    {
        await _dragDrop.DragItemAsync(_dragDrop.Draggable, _dragDrop.DropZone);
        await Expect(_dragDrop.DropZone).ToBeVisibleAsync();
    }
}
