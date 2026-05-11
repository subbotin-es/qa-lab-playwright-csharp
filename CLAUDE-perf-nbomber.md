# CLAUDE.md — Performance / NBomber (Embedded)
# File location: qa-lab-playwright-csharp/performance/CLAUDE.md

> **This file is the authoritative specification for Claude Code.**
> Read it completely before writing any scenario, any config, any CI step.
> This is an augmentation of the existing Playwright + C# + NUnit framework — not a standalone project.
> When in doubt — ask. Do not invent scenarios. Do not add NuGet packages outside this spec.

**Author:** Evgenii Subbotin
**Parent project:** qa-lab-playwright-csharp (Stack 5 — Cross-Stack Series)
**Performance tool:** NBomber
**Target:** https://subbotin.es/QA-Lab/qa-lab.html (S3 + CloudFront)
**Language:** C# / .NET 8 — natively consistent with the NUnit/Playwright ecosystem of this repo
**Version:** 1.0 | May 2026

---

## 1. What This Augmentation Does

Adds SLO compliance performance testing to the existing Playwright + C# + NUnit framework.
NBomber is the .NET-native load testing tool: C# scenarios, NuGet install,
`dotnet run` execution, JSON/HTML reports — zero toolchain friction.

**Narrative for portfolio / interviews:**
> "NBomber is niche — it barely appears in JDs. But that's not the point here.
> The point is that performance tooling follows the same ecosystem principle as
> the test framework. A .NET team picks a .NET load tool. NBomber is that tool.
> The principle is more valuable on a CV than the tool name."

**NBomber vs k6/JMeter for a .NET team:**
- No JavaScript context switch for the team
- Same IDE, same debugger, same NuGet workflow
- Scenarios are C# classes — reviewable in the same PR as production code
- `dotnet run` — identical to running the API itself

**Honest scope (CDN target):**
```
✅ p95 / p99 assertions via NBomber assertions API
✅ Step-based scenarios with think time
✅ HTML + JSON report artifacts
✅ dotnet run execution — fits .NET workflow
❌ Capacity testing — CDN target does not degrade
❌ Degradation curves — not applicable
```

---

## 2. Absolute Rules

```
NEVER use Thread.Sleep() — use TimeSpan with NBomber step config
NEVER mix NBomber scenarios with NUnit test classes
NEVER exceed 30 concurrent users in CI
NEVER add NuGet packages outside the approved list in Section 3
ALWAYS use NBomber assertions — never manual pass/fail Console.WriteLine logic
ALWAYS keep scenarios in performance/ as a separate .csproj
ALWAYS run dotnet build on performance project before pushing
ALWAYS document CDN limitation in README
ALWAYS set TreatWarningsAsErrors in performance .csproj — same standard as parent
```

---

## 3. Tech Stack + NuGet Packages

| Layer | Technology | Version | Why |
|---|---|---|---|
| Load tool | NBomber | 5.4+ | .NET native, C# scenarios, NuGet |
| HTTP client | NBomber.Http | 5.4+ | HttpClient wrapper for NBomber |
| Language | C# 12 / .NET 8 | same as parent | Ecosystem consistency |
| Report | NBomber HTML + JSON | built-in | `--report-formats Html,Json` |
| CI | GitHub Actions | current | Existing pipeline — add one job |

**NuGet packages for performance .csproj:**
```xml
<PackageReference Include="NBomber" Version="5.4.0" />
<PackageReference Include="NBomber.Http" Version="5.4.0" />
```

---

## 4. Project Structure

```
qa-lab-playwright-csharp/       ← existing repo root
└── performance/
    └── QALab.Performance/
        ├── QALab.Performance.csproj
        ├── Scenarios/
        │   ├── SmokeScenario.cs
        │   ├── BaselineScenario.cs
        │   └── ColdWarmScenario.cs
        ├── Config/
        │   └── PerformanceConfig.cs
        └── Program.cs           # entry point — select and run scenario
```

---

## 5. QALab.Performance.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="NBomber" Version="5.4.0" />
    <PackageReference Include="NBomber.Http" Version="5.4.0" />
  </ItemGroup>

</Project>
```

---

## 6. PerformanceConfig.cs — Shared Constants

```csharp
// Config/PerformanceConfig.cs
namespace QALab.Performance.Config;

public static class PerformanceConfig
{
    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("BASE_URL") ?? "https://subbotin.es";

    public const string QALabPath = "/QA-Lab/qa-lab.html";
    public const string QALabIndexPath = "/QA-Lab/index.html";

    // SLO thresholds
    public const int P95ThresholdMs = 500;
    public const int P99ThresholdMs = 1000;
    public const double ErrorRateThreshold = 1.0; // percent
}
```

---

## 7. Scenarios — Exact Implementation

### SmokeScenario.cs
```csharp
// Scenarios/SmokeScenario.cs
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using QALab.Performance.Config;

namespace QALab.Performance.Scenarios;

public static class SmokeScenario
{
    public static ScenarioProps Build()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("qa_lab_smoke", async context =>
        {
            var request = Http.CreateRequest("GET", $"{PerformanceConfig.BaseUrl}{PerformanceConfig.QALabPath}");

            var response = await Http.Send(httpClient, request);

            return response.IsError
                ? Response.Fail()
                : Response.Ok(statusCode: (int)response.StatusCode);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingConstant(copies: 5, during: TimeSpan.FromSeconds(10)),
            Simulation.KeepConstant(copies: 5, during: TimeSpan.FromSeconds(20))
        );

        return scenario;
    }

    public static IEnumerable<IAssertion> Assertions() =>
    [
        Assertion.ForScenario("qa_lab_smoke",
            s => s.Ok.Request.Percent >= 99.0),
        Assertion.ForScenario("qa_lab_smoke",
            s => s.Ok.Latency.Percent95 <= PerformanceConfig.P95ThresholdMs),
        Assertion.ForScenario("qa_lab_smoke",
            s => s.Ok.Latency.Percent99 <= PerformanceConfig.P99ThresholdMs),
    ];
}
```

### BaselineScenario.cs
```csharp
// Scenarios/BaselineScenario.cs
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using QALab.Performance.Config;

namespace QALab.Performance.Scenarios;

public static class BaselineScenario
{
    public static ScenarioProps Build()
    {
        using var httpClient = new HttpClient();

        var scenario = Scenario.Create("qa_lab_baseline", async context =>
        {
            var path = context.ScenarioInfo.IterationNumber % 2 == 0
                ? PerformanceConfig.QALabPath
                : PerformanceConfig.QALabIndexPath;

            var request = Http.CreateRequest("GET", $"{PerformanceConfig.BaseUrl}{path}");
            var response = await Http.Send(httpClient, request);

            return response.IsError
                ? Response.Fail()
                : Response.Ok(statusCode: (int)response.StatusCode);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingConstant(copies: 10, during: TimeSpan.FromSeconds(15)),
            Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(45))
        );

        return scenario;
    }

    public static IEnumerable<IAssertion> Assertions() =>
    [
        Assertion.ForScenario("qa_lab_baseline",
            s => s.Ok.Request.Percent >= 99.0),
        Assertion.ForScenario("qa_lab_baseline",
            s => s.Ok.Latency.Percent95 <= PerformanceConfig.P95ThresholdMs),
    ];
}
```

### ColdWarmScenario.cs
```csharp
// Scenarios/ColdWarmScenario.cs
using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using QALab.Performance.Config;

namespace QALab.Performance.Scenarios;

/// <summary>
/// Sequential cold/warm CDN comparison.
/// Single user, two tagged requests per iteration.
/// </summary>
public static class ColdWarmScenario
{
    public static ScenarioProps Build()
    {
        using var httpClient = new HttpClient();
        var target = $"{PerformanceConfig.BaseUrl}{PerformanceConfig.QALabPath}";

        var scenario = Scenario.Create("cdn_cold_warm", async context =>
        {
            // Cold hit
            var cold = Http.CreateRequest("GET", target);
            var coldResponse = await Http.Send(httpClient, cold);
            if (coldResponse.IsError) return Response.Fail("cold_hit failed");

            // Warm hit
            var warm = Http.CreateRequest("GET", target);
            var warmResponse = await Http.Send(httpClient, warm);

            return warmResponse.IsError
                ? Response.Fail("warm_hit failed")
                : Response.Ok();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.IterationsForConstant(copies: 1, iterations: 10)
        );

        return scenario;
    }
}
```

### Program.cs
```csharp
// Program.cs
using NBomber.CSharp;
using QALab.Performance.Scenarios;

var scenarioArg = args.Length > 0 ? args[0] : "smoke";

NBomberRunner
    .RegisterScenarios(scenarioArg switch
    {
        "baseline" => BaselineScenario.Build(),
        "cold-warm" => ColdWarmScenario.Build(),
        _ => SmokeScenario.Build(),
    })
    .WithReportFormats(ReportFormat.Html, ReportFormat.Json)
    .WithReportFolder("performance-results")
    .WithAssertions(scenarioArg switch
    {
        "baseline" => BaselineScenario.Assertions().ToArray(),
        _ => SmokeScenario.Assertions().ToArray(),
    })
    .Run();
```

---

## 8. CI Integration — Add to Existing Pipeline

```yaml
  performance:
    name: NBomber SLO Check
    runs-on: ubuntu-latest
    needs: test          # run after NUnit tests pass
    timeout-minutes: 15

    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'

      - name: Restore performance project
        run: dotnet restore performance/QALab.Performance/QALab.Performance.csproj

      - name: Build performance project
        run: |
          dotnet build performance/QALab.Performance/QALab.Performance.csproj \
            --no-restore --configuration Release

      - name: Run NBomber smoke scenario
        run: |
          dotnet run --project performance/QALab.Performance \
            --configuration Release -- smoke
        env:
          BASE_URL: https://subbotin.es

      - name: Run NBomber baseline scenario (main only)
        if: github.ref == 'refs/heads/main'
        run: |
          dotnet run --project performance/QALab.Performance \
            --configuration Release -- baseline
        env:
          BASE_URL: https://subbotin.es

      - name: Upload NBomber reports
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: nbomber-reports
          path: performance-results/
          retention-days: 14
```

---

## 9. Infrastructure Setup — Step by Step

### Step 1: Create project structure

```bash
# From repo root
mkdir -p performance/QALab.Performance/Scenarios
mkdir -p performance/QALab.Performance/Config
```

### Step 2: Create .csproj per Section 5

### Step 3: Restore and build

```bash
dotnet restore performance/QALab.Performance/QALab.Performance.csproj
dotnet build performance/QALab.Performance/QALab.Performance.csproj --configuration Release
```

### Step 4: Run locally

```bash
# Smoke
dotnet run --project performance/QALab.Performance -- smoke

# Baseline
dotnet run --project performance/QALab.Performance -- baseline

# Cold/warm
dotnet run --project performance/QALab.Performance -- cold-warm

# Report appears in performance-results/ directory
```

---

## 10. Definition of Done

```
□ dotnet build — zero warnings (TreatWarningsAsErrors)
□ All 3 scenarios run locally — assertions pass
□ performance/QALab.Performance/README.md with honest scope
□ CI job added — does not affect existing NUnit jobs
□ HTML report uploaded as artifact in CI
□ Commit message: perf(nbomber): add SLO compliance scenarios for QA Lab CDN
```

---

*End of CLAUDE.md*
*Version: 1.0 | Author: Evgenii Subbotin | Augmentation: NBomber → qa-lab-playwright-csharp*
*May 2026*
