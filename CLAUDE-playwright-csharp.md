# CLAUDE.md — QA Lab: Playwright + C# + NUnit + HTML Reporter

> **This file is the authoritative specification for Claude Code.**
> Read it completely before writing any test, any page object, any config.
> Every architectural decision documented here has a rationale — don't override without explicit instruction.
> When in doubt — ask. Do not invent test cases. Do not add NuGet packages not listed here.

**Author:** Evgenii Subbotin
**Project:** QA Lab Cross-Stack Series — Stack 5: Playwright + C#
**Target:** https://subbotin.es/QA-Lab/qa-lab.html
**Stack:** Microsoft.Playwright · C# 12 · NUnit 3 · Playwright HTML Reporter · GitHub Actions · GitHub Pages
**Version:** 1.0 | April 2026

---

## 1. What This Project Does

Isolated Playwright + C# test framework targeting the QA Lab live environment.
Demonstrates .NET automation practices expected in enterprise Microsoft-stack shops:
strongly-typed Page Object Model, NUnit fixtures, async/await throughout, and
the built-in Playwright HTML report published to GitHub Pages — zero extra tooling.

**Designed to be shown to a hiring manager by Monday.**
The stack is minimal by design: working CI → green tests → live HTML report.
No over-engineering. No exotic NuGet packages.

**This is portfolio artefact #5 in the Cross-Stack Series:**
```
Same target (qa-lab.html) → different stacks → comparative analysis
Stack 1: Playwright + TypeScript
Stack 2: Pytest + Python
Stack 3: Selenium + Java + TestNG      ← already built
Stack 4: Cypress + JS
Stack 5: Playwright + C#               ← this project
```

**Key differentiator vs other stacks:**
- C# async/await is native — `await page.ClickAsync()` is idiomatic, not bolted on
- NUnit `[TestCase]` attribute = parametrized tests without extra libraries
- .csproj dependency management — familiar to any .NET hiring manager
- Same Playwright engine as Stack 1 (TS) — clean apples-to-apples comparison

**Test coverage scope:**
```
Buttons          → click states, disabled state assertion       ✅
Forms            → validation, field interaction, submit        ✅
Input Fields     → text, number, date, search, URL types       ✅
Checkboxes       → check/uncheck, disabled state               ✅
Radio Buttons    → selection, mutual exclusivity                ✅
Dropdowns        → SelectOptionAsync(), multiple               ✅
Tables           → cell content, row count, edit action        ✅
Alerts/Modals    → open, confirm, cancel                       ✅
Dynamic Visibility → checkbox-triggered panel reveal           ✅
Async Buttons    → loading → success/error state transitions   ✅
IFrames          → FrameLocator — full support                 ✅
Drag & Drop      → DragAndDropAsync()                          ✅
Slider           → FillAsync() on input[type=range]            ✅
```

---

## 2. Absolute Rules — Read Before Every Task

```
NEVER use Task.Delay() or Thread.Sleep() — use Playwright's built-in auto-waiting
NEVER hardcode base URLs — always use _baseUrl from PlaywrightFixture
NEVER write assertions inside Page Object methods — they return ILocator or values
NEVER use dynamic or var excessively — prefer explicit types
NEVER commit .env files or secrets — use environment variables in CI
NEVER use sync-over-async (Result, .Wait()) — everything must be properly awaited
ALWAYS mark test class with [Parallelizable(ParallelScope.Self)]
ALWAYS inherit test classes from PlaywrightTest (Microsoft.Playwright.NUnit)
ALWAYS use async Task return type on every test method
ALWAYS use Expect() for assertions — not Assert.That() on raw strings
ALWAYS add [Category("smoke")] or [Category("regression")] to every test
ALWAYS run dotnet build before pushing — zero warnings required
KEEP Page Objects as classes with IPage dependency — no static state
```

---

## 3. Tech Stack

| Layer | Technology | Version | Why |
|---|---|---|---|
| Test framework | NUnit | 3.14+ | Standard .NET test framework, CI integration out of the box |
| Browser automation | Microsoft.Playwright | 1.44+ | Official .NET binding, same engine as Stack 1 |
| NUnit adapter | Microsoft.Playwright.NUnit | 1.44+ | Provides PlaywrightTest base class, IPage fixture |
| Reporting | Playwright HTML Reporter | built-in | Zero config, professional output, GitHub Pages ready |
| Language | C# | 12 (.NET 8) | LTS, modern features, async/await native |
| CI/CD | GitHub Actions | current | Free 2000 min/month |
| Report hosting | GitHub Pages | current | Free, one-step deploy from CI artifact |

**No Allure. No SpecFlow. No Selenium. No extra reporting NuGet packages.**
Budget target: $0/month (all free tiers).

---

## 4. Repository Structure

```
qa-lab-playwright-csharp/
├── QALab.Tests/
│   ├── QALab.Tests.csproj
│   ├── Tests/
│   │   ├── ButtonsTests.cs
│   │   ├── FormsTests.cs
│   │   ├── InputsTests.cs
│   │   ├── CheckboxesTests.cs
│   │   ├── DropdownsTests.cs
│   │   ├── TablesTests.cs
│   │   ├── ModalsTests.cs
│   │   ├── DynamicVisibilityTests.cs
│   │   ├── AsyncButtonsTests.cs
│   │   ├── IFrameTests.cs
│   │   ├── DragDropTests.cs
│   │   └── SliderTests.cs
│   ├── Pages/
│   │   ├── QALabPage.cs               # Base page — navigation, shared locators
│   │   ├── ButtonsSection.cs
│   │   ├── FormsSection.cs
│   │   ├── InputsSection.cs
│   │   ├── CheckboxesSection.cs
│   │   ├── DropdownsSection.cs
│   │   ├── TablesSection.cs
│   │   ├── ModalsSection.cs
│   │   ├── DynamicVisibilitySection.cs
│   │   ├── AsyncButtonsSection.cs
│   │   ├── IFrameSection.cs
│   │   └── DragDropSection.cs
│   ├── Fixtures/
│   │   └── PlaywrightFixture.cs        # Base test class — IPage, base URL, config
│   ├── Models/
│   │   └── FormData.cs                 # C# records for test data
│   └── .runsettings                    # NUnit + Playwright config
├── .github/
│   └── workflows/
│       └── ci.yml
├── playwright.config.json              # Playwright reporter config
├── .gitignore
└── README.md
```

---

## 5. QALab.Tests.csproj — Exact Dependencies

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <IsPackable>false</IsPackable>
    <!-- Required for Playwright NUnit integration -->
    <IsTestProject>true</IsTestProject>
    <!-- Emit compiler warnings as errors — no silent bad code -->
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.Playwright.NUnit" Version="1.44.0" />
    <PackageReference Include="NUnit" Version="3.14.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
  </ItemGroup>

  <!-- Copy .runsettings to output — required for NUnit parallel config -->
  <ItemGroup>
    <None Update=".runsettings">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>

</Project>
```

---

## 6. Models/FormData.cs — C# Records

```csharp
// Models/FormData.cs
namespace QALab.Tests.Models;

// C# records — immutable, value-equality, concise syntax
public record RegistrationFormData(
    string FullName,
    string Email,
    int Age,
    string Phone
);

public record TableRow(
    string Id,
    string Name,
    string Email,
    string Status
);

// Enum for async button states
public enum AsyncButtonState
{
    Default,
    Loading,
    Success,
    Error
}
```

---

## 7. Fixtures/PlaywrightFixture.cs — Base Test Class

```csharp
// Fixtures/PlaywrightFixture.cs
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace QALab.Tests.Fixtures;

/// <summary>
/// Base class for all QA Lab tests.
/// Inherits from PlaywrightTest which provides IPage, IBrowser, IBrowserContext.
/// Sets base URL from environment variable with fallback.
/// </summary>
[Parallelizable(ParallelScope.Self)]
public class PlaywrightFixture : PageTest
{
    protected string BaseUrl { get; private set; } = string.Empty;

    [SetUp]
    public async Task SetUpAsync()
    {
        BaseUrl = Environment.GetEnvironmentVariable("BASE_URL")
                  ?? "https://subbotin.es";

        // Navigate to QA Lab before each test
        await Page.GotoAsync($"{BaseUrl}/QA-Lab/qa-lab.html");

        // Verify page loaded
        await Expect(Page).ToHaveTitleAsync(new Regex("QA Lab"));
    }

    // Override BrowserNewContextOptions to set viewport
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1280, Height = 800 },
            BaseURL = Environment.GetEnvironmentVariable("BASE_URL") ?? "https://subbotin.es",
        };
    }
}
```

---

## 8. Page Object Pattern — Exact Standard

```csharp
// Pages/FormsSection.cs
using Microsoft.Playwright;

namespace QALab.Tests.Pages;

/// <summary>
/// Page object for the Registration Form section of QA Lab.
/// All methods return Task — no assertions inside.
/// Assertions live in test classes only.
/// </summary>
public class FormsSection
{
    private readonly IPage _page;

    // Locators declared as properties — lazy evaluation
    private ILocator FullNameInput => _page.Locator("input[placeholder='Full Name']");
    private ILocator EmailInput    => _page.Locator("input[type='email']").First;
    private ILocator AgeInput      => _page.Locator("input[type='number']").First;
    private ILocator PhoneInput    => _page.Locator("input[type='tel']");

    // Public for test assertions
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

    // Scroll section into view — used in SetUp
    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#forms").ScrollIntoViewIfNeededAsync();
    }
}
```

---

## 9. Test Class Pattern — NUnit + Playwright

```csharp
// Tests/FormsTests.cs
using Microsoft.Playwright;
using NUnit.Framework;
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

        // Assertion in test — not in page object
        await Expect(_forms.RegisterButton).ToBeVisibleAsync();
    }

    [Test]
    [Category("regression")]
    [Description("Empty submit should trigger browser validation")]
    public async Task EmptyFormSubmit_ShouldTriggerValidation()
    {
        await _forms.SubmitAsync();
        // First required field should receive focus
        await Expect(Page.Locator("input[placeholder='Full Name']")).ToBeFocusedAsync();
    }

    // NUnit [TestCase] — parametrized without extra libraries
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
```

---

## 10. IFrame Handling

```csharp
// Pages/IFrameSection.cs
using Microsoft.Playwright;

namespace QALab.Tests.Pages;

public class IFrameSection
{
    private readonly IPage _page;

    // C# Playwright uses FrameLocator — same API as TS
    private IFrameLocator Frame => _page.FrameLocator("#iframes iframe");

    public ILocator InnerHeading => Frame.Locator("h1, h2, h3").First;

    public IFrameSection(IPage page)
    {
        _page = page;
    }

    public async Task<string> GetInnerHeadingTextAsync()
    {
        return await InnerHeading.InnerTextAsync();
    }

    public async Task ScrollIntoViewAsync()
    {
        await _page.Locator("#iframes").ScrollIntoViewIfNeededAsync();
    }
}
```

---

## 11. .runsettings — NUnit + Playwright Parallel Config

```xml
<?xml version="1.0" encoding="utf-8"?>
<RunSettings>
  <NUnit>
    <!-- Run test classes in parallel -->
    <NumberOfTestWorkers>4</NumberOfTestWorkers>
  </NUnit>
  <Playwright>
    <BrowserName>chromium</BrowserName>
    <LaunchOptions>
      <Headless>true</Headless>
    </LaunchOptions>
  </Playwright>
</RunSettings>
```

---

## 12. playwright.config.json — HTML Reporter

```json
{
  "reporter": [
    ["html", { "outputFolder": "playwright-report", "open": "never" }],
    ["list"]
  ]
}
```

> **Note:** Playwright for .NET reads reporter config differently from the JS version.
> The HTML report is generated via the `PLAYWRIGHT_HTML_REPORT` env variable in CI,
> or by passing `--reporter html` to `pwsh bin/Debug/net8.0/playwright.ps1` manually.
> In CI we use the dotnet test output + the built-in reporter configured via env vars (Section 13).

---

## 13. CI/CD Pipeline

```yaml
# .github/workflows/ci.yml
name: QA Lab — Playwright C# + HTML Report

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]
  schedule:
    - cron: '0 7 * * 1'   # Monday 07:00 UTC — ready before your standup

jobs:
  test:
    name: Run Playwright C# Tests
    runs-on: ubuntu-latest
    timeout-minutes: 30

    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build — zero warnings
        run: dotnet build --no-restore --configuration Release

      - name: Install Playwright browsers
        run: pwsh QALab.Tests/bin/Release/net8.0/playwright.ps1 install --with-deps chromium

      - name: Run smoke tests
        run: |
          dotnet test --no-build --configuration Release \
            --filter "Category=smoke" \
            --settings QALab.Tests/.runsettings \
            --logger "trx;LogFileName=smoke-results.trx"
        env:
          BASE_URL: https://subbotin.es
          PLAYWRIGHT_HTML_REPORT: playwright-report

      - name: Run full regression
        if: github.ref == 'refs/heads/main'
        run: |
          dotnet test --no-build --configuration Release \
            --settings QALab.Tests/.runsettings \
            --logger "trx;LogFileName=full-results.trx"
        env:
          BASE_URL: https://subbotin.es
          PLAYWRIGHT_HTML_REPORT: playwright-report

      - name: Generate Playwright HTML report
        if: always()
        run: |
          pwsh QALab.Tests/bin/Release/net8.0/playwright.ps1 show-report playwright-report --port 0 || true
          # HTML report is already in playwright-report/ from test run

      - name: Upload HTML report artifact
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-html-report
          path: playwright-report/
          retention-days: 30

      - name: Upload TRX results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: trx-results
          path: "**/*.trx"
          retention-days: 14

      - name: Deploy report to GitHub Pages
        if: github.ref == 'refs/heads/main' && always()
        uses: peaceiris/actions-gh-pages@v4
        with:
          github_token: ${{ secrets.GITHUB_TOKEN }}
          publish_dir: ./playwright-report
          destination_dir: report
```

---

## 14. Infrastructure Setup — Step by Step

### Step 1: Prerequisites

```bash
# .NET 8 SDK
# Download: https://dotnet.microsoft.com/download/dotnet/8
dotnet --version   # 8.0.x required

# PowerShell Core (required for Playwright browser install on macOS/Linux)
brew install --cask powershell   # macOS
# or: https://github.com/PowerShell/PowerShell/releases

pwsh --version     # 7.x
```

### Step 2: Create Project

```bash
# Create solution + test project
mkdir qa-lab-playwright-csharp && cd qa-lab-playwright-csharp

dotnet new nunit -n QALab.Tests --framework net8.0
cd QALab.Tests

# Replace auto-generated .csproj with exact content from Section 5
# Then restore
dotnet restore

# Install Playwright browsers
dotnet build --configuration Release
pwsh bin/Release/net8.0/playwright.ps1 install chromium
pwsh bin/Release/net8.0/playwright.ps1 install-deps chromium
```

### Step 3: Register Accounts

**GitHub** (existing):
- Repo: `qa-lab-playwright-csharp` — create at https://github.com/new
- Enable GitHub Pages: Settings → Pages → Branch: `gh-pages` / root

No other accounts needed. Zero paid services.

### Step 4: GitHub Repository Setup

```bash
cd ..   # back to qa-lab-playwright-csharp root
git init

cat > .gitignore << 'EOF'
bin/
obj/
.vs/
*.user
playwright-report/
test-results/
**/*.trx
.DS_Store
*.suo
.idea/
EOF

git add .
git commit -m "chore: initial setup — Playwright + C# + NUnit"
git remote add origin https://github.com/YOUR_USERNAME/qa-lab-playwright-csharp.git
git push -u origin main
```

### Step 5: Create Directory Structure

```bash
cd QALab.Tests
mkdir Tests Pages Fixtures Models

# Remove the auto-generated UnitTest1.cs
rm UnitTest1.cs

# Create all files per Section 4 repository structure
```

### Step 6: Verify Local Run

```bash
# From repo root
dotnet build --configuration Release

# Run smoke tests
dotnet test --configuration Release \
  --filter "Category=smoke" \
  --settings QALab.Tests/.runsettings

# Open HTML report (generated in playwright-report/)
# On macOS:
open playwright-report/index.html
```

---

## 15. C# vs TypeScript Playwright — Portfolio Talking Point

Built into README — frames this project's purpose for the hiring manager:

| Aspect | Playwright C# (Stack 5) | Playwright TS (Stack 1) |
|---|---|---|
| Language paradigm | Strongly typed, compiled | Strongly typed, transpiled |
| Async model | Native async/await (Task) | Native async/await (Promise) |
| Parametrized tests | `[TestCase]` attribute | `test.each()` |
| Package manager | NuGet / .csproj | npm / package.json |
| Test framework | NUnit | Playwright Test (built-in) |
| Reporting | Playwright HTML (built-in) | Allure or Playwright HTML |
| CI install time | dotnet restore (~20s) | npm ci (~30s) |
| Enterprise fit | High — any .NET shop | Growing |
| IFrame support | Full (FrameLocator) | Full (frameLocator) |

*Both stacks use the same underlying Playwright engine — test reliability is identical.*
*The choice between them is purely about language ecosystem and team context.*

---

## 16. Known Limitations & Trade-offs

| Topic | Decision | Rationale |
|---|---|---|
| PowerShell dependency | Required for browser install | `playwright.ps1` is the official .NET browser installer — no alternative |
| Single browser in CI | Chromium only | Firefox and WebKit installable but add CI time; Chromium covers 95% of failures |
| HTML report persistence | GitHub Pages overwrites on each push | Acceptable for portfolio — history visible in Actions artifacts (30 days) |
| TreatWarningsAsErrors | Enabled | Demonstrates .NET code quality discipline expected in enterprise |
| Parallel scope | `ParallelScope.Self` | Each test class runs in parallel but tests within a class are sequential — safe for Page state |

---

## 17. Definition of Done — Per Task

```
□ dotnet build --configuration Release — zero errors, zero warnings
□ Test has [Category("smoke")] or [Category("regression")]
□ Test method returns async Task
□ Test has at minimum one Expect(...) assertion
□ Page Object method returns Task or ILocator — no assertions inside
□ No Task.Delay() or Thread.Sleep() anywhere in diff
□ Smoke tests pass locally before push
□ Commit message: test(section-name): describe what is tested
```

---

## 18. Day-by-Day Prompts for Claude Code

### DAY 1 PROMPT — Infrastructure + Scaffolding

```
Read CLAUDE.md Sections 14, 5, 7 completely before starting.

Goal: Project builds, Playwright browsers installed, zero test code.

Tasks in order:
1. Verify dotnet --version (8.0.x required) and pwsh --version (7.x required)
2. Create qa-lab-playwright-csharp directory
3. dotnet new nunit -n QALab.Tests --framework net8.0
4. Replace QALab.Tests.csproj with exact content from Section 5
5. dotnet restore → must succeed
6. dotnet build --configuration Release → must succeed
7. pwsh bin/Release/net8.0/playwright.ps1 install chromium
8. Create directory structure: Tests/ Pages/ Fixtures/ Models/
9. Remove auto-generated UnitTest1.cs
10. Create Models/FormData.cs (Section 6)
11. Create Fixtures/PlaywrightFixture.cs (Section 7)
12. Create .runsettings (Section 11)
13. Create .gitignore, initial commit, push to GitHub

After completing:
- dotnet build --configuration Release → zero warnings (TreatWarningsAsErrors = true)
- dotnet test --list-tests → shows 0 tests (no test files yet, OK)

Do NOT write any test files today.
```

---

### DAY 2 PROMPT — Page Objects

```
Read CLAUDE.md Sections 8, 10 before starting.

Goal: All Page Objects written with async Task methods.

Implement in order:
1. Pages/QALabPage.cs — GotoAsync(), ScrollToSectionAsync()
2. Pages/ButtonsSection.cs — ClickPrimaryAsync(), IsPrimaryButtonVisible(), IsDisabledButtonDisabled()
3. Pages/FormsSection.cs — exact pattern from Section 8
4. Pages/InputsSection.cs — FillTextAsync(), FillNumberAsync(), FillDateAsync()
5. Pages/CheckboxesSection.cs — CheckAsync(), UncheckAsync(), IsCheckedAsync()
6. Pages/DropdownsSection.cs — SelectByValueAsync() using SelectOptionAsync()
7. Pages/TablesSection.cs — GetRowCountAsync(), GetCellTextAsync(row, col)
8. Pages/ModalsSection.cs — OpenAsync(), ConfirmAsync(), CancelAsync()
9. Pages/DynamicVisibilitySection.cs — ToggleAsync(), IsPanelVisibleAsync()
10. Pages/AsyncButtonsSection.cs — ClickAsync(), WaitForStateAsync(expectedText)
11. Pages/IFrameSection.cs — exact pattern from Section 10
12. Pages/DragDropSection.cs — DragItemAsync(source, target)

After each file: dotnet build --configuration Release — fix all warnings before continuing.
Rule: every method is async Task or async Task<T>. No sync methods.
```

---

### DAY 3 PROMPT — Test Classes

```
Read CLAUDE.md Sections 9, 16, 17 before starting.

Goal: All test classes written, smoke tests pass locally.

Write in order:
1. Tests/ButtonsTests.cs — [Category("smoke")] on primary button test
2. Tests/FormsTests.cs — exact pattern from Section 9, include [TestCase] examples
3. Tests/InputsTests.cs
4. Tests/CheckboxesTests.cs
5. Tests/DropdownsTests.cs
6. Tests/TablesTests.cs
7. Tests/ModalsTests.cs
8. Tests/DynamicVisibilityTests.cs
9. Tests/AsyncButtonsTests.cs
10. Tests/IFrameTests.cs
11. Tests/DragDropTests.cs
12. Tests/SliderTests.cs

Run after each:
dotnet test --configuration Release --filter "Category=smoke" --settings QALab.Tests/.runsettings

All smoke tests at end:
dotnet test --configuration Release --filter "Category=smoke"
Expected: all green.
```

---

### DAY 4 PROMPT — CI + README (Monday delivery)

```
Goal: CI green, HTML report live on GitHub Pages, README shows hiring manager the value.

Tasks:
1. Create .github/workflows/ci.yml (Section 13)
2. Push to main → verify Actions run → verify report deploys to gh-pages
3. Get report URL: https://YOUR_USERNAME.github.io/qa-lab-playwright-csharp/report/
4. Write README.md:
   - One sentence: what this demonstrates
   - GitHub Actions badge
   - Live HTML Report link (GitHub Pages URL)
   - Stack table (Section 15)
   - Run locally: 3 commands
   - C# vs TS comparison table (Section 15)
   - Known limitations (Section 16)
   - Cross-Stack Series links (all 5 repos)
5. Push README → verify badge shows green

Monday delivery checklist:
□ https://github.com/YOUR_USERNAME/qa-lab-playwright-csharp → public repo visible
□ GitHub Actions badge: green
□ Live HTML report URL in README: works, shows test results
□ dotnet build --configuration Release: zero warnings in CI logs
□ All smoke tests: green in CI
□ README is readable by a non-technical hiring manager in 30 seconds
```

---

## 19. Common Errors and How to Fix Them

**`pwsh: command not found`**
→ PowerShell Core not installed. On macOS: `brew install --cask powershell`. On Ubuntu CI: use `pwsh` from `actions/setup-dotnet` — it's pre-installed on ubuntu-latest runners.

**`dotnet build: error — TreatWarningsAsErrors`**
→ A compiler warning is blocking the build. Read the warning, fix it. Common cause: nullable reference type not handled. Add `!` operator or null check.

**`Playwright.PlaywrightException: Browser not found`**
→ Browsers not installed. Run: `pwsh bin/Release/net8.0/playwright.ps1 install chromium`. In CI, check the install step ran before the test step.

**`NUnit: No tests found matching filter 'Category=smoke'`**
→ `[Category("smoke")]` attribute missing on tests. Check test class and method both have appropriate `[Category]`.

**`InvalidOperationException: Page is not available`**
→ `Page` property accessed before `SetUp` runs, or test class does not inherit `PlaywrightFixture`. Check class declaration: `public class YourTests : PlaywrightFixture`.

**`System.AggregateException` wrapping TimeoutException**
→ Element not found in time. Check locator selector against live QA Lab HTML. Increase `defaultTimeout` in `ContextOptions()` if the target is slow.

**`playwright-report/ is empty after CI run`**
→ `PLAYWRIGHT_HTML_REPORT` env var not set, or tests failed before writing report. Check both smoke and full test steps set `PLAYWRIGHT_HTML_REPORT: playwright-report`.

**`GitHub Pages 404`**
→ Pages not enabled on `gh-pages` branch. Settings → Pages → Source: Deploy from branch → `gh-pages` → `/ (root)`.

**`Parallelism causes Page state conflicts`**
→ Ensure `[Parallelizable(ParallelScope.Self)]` is on the class, not `ParallelScope.All`. Each test class gets its own `IPage` instance from `PlaywrightTest`.

---

## 20. Branching Strategy

```
main      → production (triggers CI + report deploy to GitHub Pages)
feat/     → new test sections
fix/      → broken test fixes
chore/    → config, dependency updates
```

Commit message format: `type(scope): description`
Examples:
- `test(forms): add TestCase parametrization for email validation`
- `fix(iframe): update FrameLocator selector after QA Lab DOM change`
- `chore(deps): bump Microsoft.Playwright.NUnit to 1.45`
- `docs(readme): add live report link and CI badge`

---

*End of CLAUDE.md*
*Version: 1.0 | Author: Evgenii Subbotin | Project: QA Lab Cross-Stack Series — Stack 5*
*April 2026*
