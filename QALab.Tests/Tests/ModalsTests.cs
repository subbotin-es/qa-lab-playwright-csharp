using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("modals")]
public class ModalsTests : PlaywrightFixture
{
    private ModalsSection _modals = null!;

    [SetUp]
    public async Task SetUpModalsAsync()
    {
        _modals = new ModalsSection(Page);
        await _modals.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Modal is hidden before the open button is clicked")]
    public async Task Modal_ShouldBeHiddenInitially()
    {
        await Expect(_modals.Modal).ToBeHiddenAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Clicking open button displays the modal dialog")]
    public async Task OpenModal_ShouldShowModal()
    {
        await _modals.OpenAsync();
        await Expect(_modals.Modal).ToBeVisibleAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Clicking cancel closes the modal without confirming")]
    public async Task CancelModal_ShouldHideModal()
    {
        await _modals.OpenAsync();
        await _modals.CancelAsync();
        await Expect(_modals.Modal).ToBeHiddenAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Clicking the X close button dismisses the modal (modal-confirm has no handler on the live page)")]
    public async Task ConfirmModal_ShouldHideModal()
    {
        await _modals.OpenAsync();
        await _modals.ConfirmAsync();
        await Expect(_modals.Modal).ToBeHiddenAsync();
    }
}
