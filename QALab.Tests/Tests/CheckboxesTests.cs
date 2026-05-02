using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("checkboxes")]
public class CheckboxesTests : PlaywrightFixture
{
    private CheckboxesSection _checkboxes = null!;

    [SetUp]
    public async Task SetUpCheckboxesAsync()
    {
        _checkboxes = new CheckboxesSection(Page);
        await _checkboxes.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Checking an unchecked checkbox marks it as checked")]
    public async Task CheckCheckbox_ShouldBeChecked()
    {
        await _checkboxes.CheckAsync(0);
        await Expect(_checkboxes.Checkboxes.Nth(0)).ToBeCheckedAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Option 3 (index 2) is pre-checked — unchecking it should remove the check")]
    public async Task UncheckPreCheckedCheckbox_ShouldBeUnchecked()
    {
        await _checkboxes.UncheckAsync(2);
        await Expect(_checkboxes.Checkboxes.Nth(2)).Not.ToBeCheckedAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Option 4 (index 3) is disabled and cannot be interacted with")]
    public async Task DisabledCheckbox_ShouldBeDisabled()
    {
        await Expect(_checkboxes.Checkboxes.Nth(3)).ToBeDisabledAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Selecting a radio button marks it as checked")]
    public async Task SelectRadioButton_ShouldBeChecked()
    {
        await _checkboxes.SelectRadioAsync(0);
        await Expect(_checkboxes.RadioButtons.Nth(0)).ToBeCheckedAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Selecting a second radio button deselects the first — mutual exclusivity")]
    public async Task RadioButtons_ShouldBeMutuallyExclusive()
    {
        await _checkboxes.SelectRadioAsync(0);
        await _checkboxes.SelectRadioAsync(1);

        await Expect(_checkboxes.RadioButtons.Nth(1)).ToBeCheckedAsync();
        await Expect(_checkboxes.RadioButtons.Nth(0)).Not.ToBeCheckedAsync();
    }
}
