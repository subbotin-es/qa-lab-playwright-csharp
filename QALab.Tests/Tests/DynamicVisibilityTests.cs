using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("dynamic-visibility")]
public class DynamicVisibilityTests : PlaywrightFixture
{
    private DynamicVisibilitySection _dynamicVisibility = null!;

    [SetUp]
    public async Task SetUpDynamicVisibilityAsync()
    {
        _dynamicVisibility = new DynamicVisibilitySection(Page);
        await _dynamicVisibility.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Secret panel is hidden before the toggle checkbox is checked")]
    public async Task SecretPanel_ShouldBeHiddenInitially()
    {
        await Expect(_dynamicVisibility.SecretPanel).ToBeHiddenAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Checking the toggle checkbox reveals the secret panel")]
    public async Task ToggleCheckbox_ShouldRevealPanel()
    {
        await _dynamicVisibility.ToggleAsync();
        await Expect(_dynamicVisibility.SecretPanel).ToBeVisibleAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Toggling the checkbox twice hides the panel again")]
    public async Task TogglingTwice_ShouldHidePanel()
    {
        await _dynamicVisibility.ToggleAsync();
        await _dynamicVisibility.ToggleAsync();
        await Expect(_dynamicVisibility.SecretPanel).ToBeHiddenAsync();
    }
}
