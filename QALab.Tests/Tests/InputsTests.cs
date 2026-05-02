using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("inputs")]
public class InputsTests : PlaywrightFixture
{
    private InputsSection _inputs = null!;

    [SetUp]
    public async Task SetUpInputsAsync()
    {
        _inputs = new InputsSection(Page);
        await _inputs.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Text input accepts and retains typed value")]
    public async Task FillTextInput_ShouldHaveValue()
    {
        await _inputs.FillTextAsync("Hello World");
        await Expect(_inputs.TextInput).ToHaveValueAsync("Hello World");
    }

    [Test]
    [Category("regression")]
    [Description("Number input accepts numeric value")]
    public async Task FillNumberInput_ShouldHaveValue()
    {
        await _inputs.FillNumberAsync("42");
        await Expect(_inputs.NumberInput).ToHaveValueAsync("42");
    }

    [Test]
    [Category("regression")]
    [Description("Date input accepts ISO date string")]
    public async Task FillDateInput_ShouldHaveValue()
    {
        await _inputs.FillDateAsync("2024-06-15");
        await Expect(_inputs.DateInput).ToHaveValueAsync("2024-06-15");
    }

    [Test]
    [Category("regression")]
    [Description("Search input accepts search term")]
    public async Task FillSearchInput_ShouldHaveValue()
    {
        await _inputs.FillSearchAsync("playwright");
        await Expect(_inputs.SearchInput).ToHaveValueAsync("playwright");
    }

    [Test]
    [Category("regression")]
    [Description("URL input accepts a valid URL string")]
    public async Task FillUrlInput_ShouldHaveValue()
    {
        await _inputs.FillUrlAsync("https://example.com");
        await Expect(_inputs.UrlInput).ToHaveValueAsync("https://example.com");
    }
}
