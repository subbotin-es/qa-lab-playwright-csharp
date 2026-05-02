using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("dropdowns")]
public class DropdownsTests : PlaywrightFixture
{
    private DropdownsSection _dropdowns = null!;

    [SetUp]
    public async Task SetUpDropdownsAsync()
    {
        _dropdowns = new DropdownsSection(Page);
        await _dropdowns.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Selecting Canada by visible text updates the single-select dropdown")]
    public async Task SelectByLabel_ShouldUpdateSelection()
    {
        await _dropdowns.SelectByLabelAsync("Canada");
        await Expect(_dropdowns.SingleDropdown).ToHaveValueAsync("canada");
    }

    [Test]
    [Category("regression")]
    [Description("Selecting a different country replaces the current selection")]
    public async Task SelectDifferentLabel_ShouldReplaceSelection()
    {
        await _dropdowns.SelectByLabelAsync("Canada");
        await _dropdowns.SelectByLabelAsync("United Kingdom");
        await Expect(_dropdowns.SingleDropdown).ToContainTextAsync("United Kingdom");
    }

    [Test]
    [Category("regression")]
    [Description("Multi-select dropdown accepts multiple simultaneous selections by visible text")]
    public async Task MultiSelect_ShouldSelectMultipleValues()
    {
        await _dropdowns.SelectMultipleByLabelAsync("Testing", "Design");
        int selectedCount = await _dropdowns.MultiDropdown
            .Locator("option:checked").CountAsync();
        Assert.That(selectedCount, Is.EqualTo(2));
    }

    [TestCase("Canada")]
    [TestCase("Australia")]
    [Category("regression")]
    public async Task SelectByLabel_MultipleCases(string country)
    {
        await _dropdowns.SelectByLabelAsync(country);
        await Expect(_dropdowns.SingleDropdown).ToContainTextAsync(country);
    }
}
