# QALab.Performance — NBomber SLO Scenarios

NBomber load testing for the QA Lab CDN target (`https://subbotin.es/QA-Lab/qa-lab.html`).
Part of the qa-lab-playwright-csharp Stack 5 (Cross-Stack) series.

## Scenarios

| Scenario | Arg | Users | Duration | Assertions |
|---|---|---|---|---|
| Smoke | `smoke` | 5 | ~30 s | p95 ≤ 500 ms, p99 ≤ 1000 ms, success ≥ 99% |
| Baseline | `baseline` | 10 | ~60 s | p95 ≤ 500 ms, success ≥ 99% |
| Cold/Warm | `cold-warm` | 1 | 10 iterations | no assertions — observational |

## Run locally

```bash
# Smoke (default)
dotnet run --project performance/QALab.Performance -- smoke

# Baseline
dotnet run --project performance/QALab.Performance -- baseline

# Cold/Warm CDN comparison
dotnet run --project performance/QALab.Performance -- cold-warm
```

Reports are written to `performance-results/` (HTML + JSON).

## Honest scope

This target is served by S3 + CloudFront (CDN). What this suite **can** verify:

- p95/p99 SLOs under moderate concurrent load
- CDN warm vs cold response comparison
- Error rate stays below 1%

What it **cannot** verify:

- Capacity limits — CDN does not degrade under 10–30 users
- Degradation curves — origin is not exposed
- Throughput saturation

These limitations are inherent to the CDN architecture, not a gap in the tooling.

## Tech stack

- **NBomber 5.4** — .NET-native load tool, C# scenarios, NuGet install
- **NBomber.Http 5.4** — HttpClient wrapper
- **.NET 8** — consistent with the parent NUnit/Playwright project
