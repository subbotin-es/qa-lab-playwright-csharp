using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("tables")]
public class TablesTests : PlaywrightFixture
{
    private TablesSection _tables = null!;

    [SetUp]
    public async Task SetUpTablesAsync()
    {
        _tables = new TablesSection(Page);
        await _tables.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Table renders with at least one row of data")]
    public async Task TableRowCount_ShouldBeGreaterThanZero()
    {
        int rowCount = await _tables.GetRowCountAsync();
        Assert.That(rowCount, Is.GreaterThan(0));
    }

    [Test]
    [Category("smoke")]
    [Description("First cell of first row returns a non-empty string")]
    public async Task GetCellText_ShouldReturnContent()
    {
        string cellText = await _tables.GetCellTextAsync(1, 1);
        Assert.That(cellText, Is.Not.Empty);
    }

    [Test]
    [Category("regression")]
    [Description("Edit button in first row is visible and clickable")]
    public async Task EditButton_ShouldBeClickable()
    {
        await Expect(Page.Locator("#tables tbody tr:nth-child(1)").GetByRole(AriaRole.Button)).ToBeVisibleAsync();
    }

    [TestCase(1, 1)]
    [TestCase(1, 2)]
    [TestCase(2, 1)]
    [Category("regression")]
    public async Task GetCellText_MultipleCells_ShouldReturnNonEmpty(int row, int col)
    {
        string cellText = await _tables.GetCellTextAsync(row, col);
        Assert.That(cellText, Is.Not.Empty);
    }
}
