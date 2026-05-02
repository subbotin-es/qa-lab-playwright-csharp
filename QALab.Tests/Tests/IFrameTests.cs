using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("iframes")]
public class IFrameTests : PlaywrightFixture
{
    private IFrameSection _iframe = null!;

    [SetUp]
    public async Task SetUpIFrameAsync()
    {
        _iframe = new IFrameSection(Page);
        await _iframe.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("IFrame content is accessible via FrameLocator and renders a heading")]
    public async Task IFrame_ShouldContainHeading()
    {
        await Expect(_iframe.InnerHeading).ToBeVisibleAsync();
    }

    [Test]
    [Category("regression")]
    [Description("IFrame heading text is non-empty — confirms frame context switch worked")]
    public async Task IFrame_HeadingText_ShouldBeNonEmpty()
    {
        string headingText = await _iframe.GetInnerHeadingTextAsync();
        Assert.That(headingText, Is.Not.Empty);
    }
}
