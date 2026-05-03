# QA Lab — Playwright + C# + NUnit

End-to-end test framework targeting the [QA Lab live demo](https://subbotin.es/QA-Lab/qa-lab.html), written in C# with Microsoft.Playwright and NUnit 3. Demonstrates the strongly-typed, async-native .NET automation style expected in enterprise Microsoft-stack shops.

[![CI](https://github.com/subbotin-es/qa-lab-playwright-csharp/actions/workflows/ci.yml/badge.svg)](https://github.com/subbotin-es/qa-lab-playwright-csharp/actions/workflows/ci.yml)

**[Live HTML Test Report →](https://subbotin-es.github.io/qa-lab-playwright-csharp/report/)**

---

## Stack

| Layer | Technology | Version |
|---|---|---|
| Test framework | NUnit | 3.14 |
| Browser automation | Microsoft.Playwright | 1.51 |
| NUnit adapter | Microsoft.Playwright.NUnit | 1.51 |
| Reporting | dorny/test-reporter + ReportGenerator HTML | — |
| Language | C# 12 / .NET 8 LTS | net8.0 |
| CI/CD | GitHub Actions | ubuntu-latest |
| Report hosting | GitHub Pages | gh-pages branch |

---

## Run Locally

```bash
# 1 — restore + build (zero warnings enforced by TreatWarningsAsErrors)
dotnet restore && dotnet build --configuration Release

# 2 — install Chromium (first time only)
pwsh QALab.Tests/bin/Release/net8.0/playwright.ps1 install chromium

# 3 — run smoke tests
dotnet test --configuration Release \
  --filter "Category=smoke" \
  --settings QALab.Tests/.runsettings
```

Run the full regression suite:

```bash
dotnet test --configuration Release --settings QALab.Tests/.runsettings
```

---

## Test Coverage

| Section | What is tested | Category |
|---|---|---|
| Buttons | Click states, disabled attribute assertion | smoke + regression |
| Forms | Valid submit, empty-submit validation, `[TestCase]` parametrization | smoke + regression |
| Inputs | text, number, date, search, url — fill + value assertion | smoke + regression |
| Checkboxes | Check/uncheck, pre-checked state, disabled state | smoke + regression |
| Radio Buttons | Selection, mutual exclusivity | smoke + regression |
| Dropdowns | `SelectOptionAsync` by label, multi-select | smoke + regression |
| Tables | Row count, cell text, edit action | smoke + regression |
| Modals | Open, confirm, cancel — visibility cycle | smoke + regression |
| Dynamic Visibility | Checkbox-triggered panel reveal + re-hide | smoke + regression |
| Async Buttons | `data-state` idle → loading → success via `ToHaveAttributeAsync` | smoke + regression |
| IFrames | `FrameLocator` heading visibility and text | smoke + regression |
| Drag & Drop | `DragToAsync` with default and explicit source/target | smoke + regression |
| Slider | `FillAsync` on `input[type=range]`, boundary values | smoke + regression |

---

## CI Pipeline

Two-stage run on every push and pull request, with a **`workflow_dispatch`** trigger for manual reruns:

| Stage | Runs on | Tests |
|---|---|---|
| Smoke | all pushes + PRs | `--filter "Category=smoke"` |
| Full regression | `main` branch only | all tests |

**Reports generated on every run:**
- **Inline check run** — [dorny/test-reporter](https://github.com/dorny/test-reporter) shows pass/fail table directly in the GitHub Actions panel and PR checks
- **HTML report** — ReportGenerator renders TRX → HTML, deployed to [GitHub Pages](https://subbotin-es.github.io/qa-lab-playwright-csharp/report/)
- **Artifacts** — TRX files (14 days) and HTML report (30 days) downloadable from the Actions run

Scheduled run: **every Monday 07:00 UTC** to catch regressions from QA Lab changes over the weekend.

---

## C# vs TypeScript Playwright — Comparison

Same Playwright engine, different language ecosystem:

| Aspect | Playwright C# (this repo) | Playwright TypeScript |
|---|---|---|
| Language paradigm | Strongly typed, compiled | Strongly typed, transpiled |
| Async model | Native `async/await` (Task) | Native `async/await` (Promise) |
| Parametrized tests | `[TestCase]` attribute — no extra libraries | `test.each()` |
| Package manager | NuGet / .csproj | npm / package.json |
| Test framework | NUnit 3 | Playwright Test (built-in) |
| Reporting | dorny inline + ReportGenerator HTML | Playwright HTML reporter |
| Enterprise fit | High — any .NET / Microsoft shop | Growing |
| IFrame support | Full (`FrameLocator`) | Full (`frameLocator`) |

*Test reliability is identical — both stacks drive the same Chromium engine.*

---

## Known Limitations

| Topic | Decision | Rationale |
|---|---|---|
| PowerShell dependency | Required for browser install | `playwright.ps1` is the official .NET browser installer |
| Single browser in CI | Chromium only | Covers 95 % of failures; Firefox/WebKit add CI time |
| HTML report persistence | GitHub Pages overwrites on each push | History visible in Actions artifacts (30 days) |
| `TreatWarningsAsErrors` | Enabled | Demonstrates .NET code quality discipline |
| Parallel scope | `ParallelScope.Self` | Classes run in parallel; tests within a class are sequential — safe for Page state |

---

## Cross-Stack Series

Same target ([qa-lab.html](https://subbotin.es/QA-Lab/qa-lab.html)), five stacks — comparative analysis:

| Stack | Repo |
|---|---|
| Stack 1: Playwright + TypeScript | *(coming soon)* |
| Stack 2: Pytest + Python | *(coming soon)* |
| Stack 3: Selenium + Java + TestNG | [qa-lab-selenium-java](https://github.com/subbotin-es/qa-lab-selenium-java) |
| Stack 4: Cypress + JavaScript | *(coming soon)* |
| **Stack 5: Playwright + C# ← this repo** | [qa-lab-playwright-csharp](https://github.com/subbotin-es/qa-lab-playwright-csharp) |

---

*Author: Evgenii Subbotin · [subbotin.es](https://subbotin.es)*
