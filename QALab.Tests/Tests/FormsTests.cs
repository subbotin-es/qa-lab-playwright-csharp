using Microsoft.Playwright;
using QALab.Tests.Fixtures;
using QALab.Tests.Models;
using QALab.Tests.Pages;

namespace QALab.Tests.Tests;

[TestFixture]
[Category("forms")]
public class FormsTests : PlaywrightFixture
{
    private FormsSection _forms = null!;

    [SetUp]
    public async Task SetUpFormsAsync()
    {
        _forms = new FormsSection(Page);
        await _forms.ScrollIntoViewAsync();
    }

    [Test]
    [Category("smoke")]
    [Description("Happy path: submit registration form with valid data")]
    public async Task ValidFormSubmit_ShouldSucceed()
    {
        var data = new RegistrationFormData(
            FullName: "John Doe",
            Email: "john@example.com",
            Age: 30,
            Phone: "+1234567890"
        );

        await _forms.FillFormAsync(data);
        await _forms.SubmitAsync();

        await Expect(_forms.RegisterButton).ToBeVisibleAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Empty submit triggers custom JS validation and shows error message in feedback element")]
    public async Task EmptyFormSubmit_ShouldTriggerValidation()
    {
        await _forms.SubmitAsync();
        // Form uses novalidate + custom JS — errors appear in #reg-feedback with class 'error'
        await Expect(Page.Locator("#reg-feedback")).ToHaveClassAsync(new System.Text.RegularExpressions.Regex("error"));
        await Expect(Page.Locator("#reg-feedback")).ToContainTextAsync("Name must be at least 2 characters");
    }

    [TestCase("John Doe",   "john@example.com",  30, "+1234567890")]
    [TestCase("Jane Smith", "jane@example.com",  25, "+9876543210")]
    [Category("regression")]
    public async Task ValidFormData_MultipleCases(string name, string email, int age, string phone)
    {
        var data = new RegistrationFormData(name, email, age, phone);
        await _forms.FillFormAsync(data);
        await _forms.SubmitAsync();
        await Expect(_forms.RegisterButton).ToBeVisibleAsync();
    }
}
