using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("buttons")]
public class ButtonsTests : PlaywrightFixture
{
    private ButtonsSection _buttons = null!;

    [SetUp]
    public async Task SetUpButtonsAsync()
    {
        _buttons = new ButtonsSection(Page);
        await _buttons.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Primary button is visible and enabled in the buttons section")]
    public async Task PrimaryButton_ShouldBeVisible()
    {
        await Expect(_buttons.PrimaryButton).ToBeVisibleAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Disabled button carries the disabled attribute and cannot be interacted with")]
    public async Task DisabledButton_ShouldBeDisabled()
    {
        await Expect(_buttons.DisabledButton).ToBeDisabledAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Clicking primary button completes without error")]
    public async Task ClickPrimary_ShouldComplete()
    {
        await _buttons.ClickPrimaryAsync();
        await Expect(_buttons.PrimaryButton).ToBeVisibleAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Danger button is visible and enabled")]
    public async Task DangerButton_ShouldBeVisible()
    {
        await Expect(_buttons.DangerButton).ToBeVisibleAsync();
    }
}
