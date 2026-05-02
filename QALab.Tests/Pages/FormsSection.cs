using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class FormsSection
{
    private readonly IPage _page;

    private ILocator FullNameInput => _page.Locator("input[placeholder='Full Name']");
    private ILocator EmailInput    => _page.Locator("input[type='email']").First;
    private ILocator AgeInput      => _page.Locator("#forms input[type='number']");
    private ILocator PhoneInput    => _page.Locator("input[type='tel']");

    public ILocator RegisterButton => _page.GetByRole(AriaRole.Button, new() { Name = "Register" });

    public FormsSection(IPage page)
    {
        _page = page;
    }

    public async Task FillFormAsync(Models.RegistrationFormData data)
    {
        await FullNameInput.FillAsync(data.FullName);
        await EmailInput.FillAsync(data.Email);
        await AgeInput.FillAsync(data.Age.ToString());
        await PhoneInput.FillAsync(data.Phone);
    }

    public async Task SubmitAsync()
    {
        await RegisterButton.ClickAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#forms").ScrollIntoViewIfNeededAsync();
    }
}
