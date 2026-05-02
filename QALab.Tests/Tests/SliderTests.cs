using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("slider")]
public class SliderTests : PlaywrightFixture
{
    private DragDropSection _dragDrop = null!;

    [SetUp]
    public async Task SetUpSliderAsync()
    {
        _dragDrop = new DragDropSection(Page);
        await _dragDrop.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Slider is visible with a default value of 50")]
    public async Task Slider_ShouldBeVisibleWithDefaultValue()
    {
        await Expect(_dragDrop.Slider).ToBeVisibleAsync();
        await Expect(_dragDrop.Slider).ToHaveValueAsync("50");
    }

    [Test]
    [Category("smoke")]
    [Description("Setting slider value via FillAsync updates the input value attribute")]
    public async Task SetSliderValue_ShouldUpdateValue()
    {
        await _dragDrop.SetSliderValueAsync("75");
        await Expect(_dragDrop.Slider).ToHaveValueAsync("75");
    }

    [TestCase("0")]
    [TestCase("100")]
    [TestCase("25")]
    [Category("regression")]
    public async Task Slider_AcceptsBoundaryAndMidValues(string value)
    {
        await _dragDrop.SetSliderValueAsync(value);
        await Expect(_dragDrop.Slider).ToHaveValueAsync(value);
    }
}
