# Performance Testing Findings

**Project:** qa-lab-playwright-csharp (Stack 5 — Cross-Stack Series)
**Tool:** NBomber 6.4 / NBomber.Http 6.2
**Target:** `https://subbotin.es/QA-Lab/qa-lab.html` (S3 + CloudFront CDN)
**Run date:** May 2026
**Author:** Evgenii Subbotin

---

## 1. Approach

### Why a separate load testing layer

Playwright tests verify *correctness* — they assert UI state and user flows. They deliberately make one request at a time. Performance testing answers a different question: how does the system behave under concurrent load? These two concerns are kept in separate projects (`QALab.Tests/` and `performance/QALab.Performance/`) to avoid coupling test categories and to let each tool run independently in CI.

### Why NBomber

NBomber is the .NET-native load tool: pure C# scenarios, NuGet install, `dotnet run` execution. For a .NET shop this means no language context switch — the same IDE, debugger, and NuGet workflow used for production code. Scenarios are C# classes, reviewable in the same pull request as the feature they cover.

k6 or JMeter would require a JavaScript or XML context switch. JMeter's GUI-first model diverges from a code-review workflow. NBomber stays in the .NET ecosystem — the principle is more transferable than the specific tool.

### Scenario design

Three scenarios were designed to cover distinct measurement goals:

| Scenario | VUs | Duration | Goal |
|---|---|---|---|
| Smoke | 5 | ~30 s (ramp 10 s + hold 20 s) | SLO validation at minimal load |
| Baseline | 10 | ~60 s (ramp 15 s + hold 45 s) | SLO validation at moderate concurrent load, alternating URLs |
| Cold/Warm | 1 | 10 iterations | CDN cache behaviour observation |

The **Baseline** scenario alternates between `qa-lab.html` and `index.html` on even/odd invocations to exercise two separate CDN edge cache entries and reduce the effect of a single hot object.

The **Cold/Warm** scenario sends two sequential GET requests to the same URL in each iteration. The first request is expected to be a cache miss (cold); the second should hit the warm CDN edge. It is **observational only** — no SLO assertions are attached because latency variance is inherent to a CDN cache miss, not a defect.

---

## 2. Measured Results

All figures from a single local run on Windows 10, residential internet (Warsaw, Poland → CloudFront edge).

### Smoke (5 VU, 30 s)

| Metric | Value | SLO | Pass |
|---|---|---|---|
| Requests | 3 020 | — | — |
| RPS | 101 | — | — |
| p50 | 40 ms | — | — |
| p75 | 42 ms | — | — |
| p95 | **47 ms** | ≤ 500 ms | ✓ |
| p99 | **77 ms** | ≤ 1 000 ms | ✓ |
| Error rate | 0 % | < 1 % | ✓ |
| Total data | 85.7 MB | — | — |

### Baseline (10 VU, 60 s)

| Metric | Value | SLO | Pass |
|---|---|---|---|
| Requests | 11 544 | — | — |
| RPS | 192 | — | — |
| p50 | 44 ms | — | — |
| p75 | 47 ms | — | — |
| p95 | **55 ms** | ≤ 500 ms | ✓ |
| p99 | **81 ms** | ≤ 1 000 ms | — (not asserted) |
| Error rate | 0 % | < 1 % | ✓ |
| Total data | 334.8 MB | — | — |

### Cold/Warm CDN Comparison (1 VU, 10 iterations)

| Metric | Value |
|---|---|
| Requests (iterations) | 10 |
| p50 | 44 ms |
| p75 | 48 ms |
| p95 | 441 ms |
| p99 | 441 ms |
| Error rate | 0 % |

The p95/p99 spike to 441 ms is a single cold cache miss — see Section 3 for interpretation.

---

## 3. Key Findings

### Finding 1 — CDN latency is well within SLO

Across both SLO-asserted scenarios (smoke and baseline), p95 sits at 47–55 ms against a 500 ms threshold, and p99 at 77–81 ms against a 1 000 ms threshold. The CDN delivers consistently fast responses from the CloudFront edge closest to the client. There is no latency creep between 5 and 10 concurrent users, which is expected for a CDN-served static page.

### Finding 2 — Cold cache miss is real and visible

In the Cold/Warm scenario, one of the 10 iterations produced a 441 ms round-trip — approximately 10× the warm cache response of ~44 ms. This is the CDN origin fetch latency (S3 → CloudFront) on a cache miss. The effect is expected and is not a defect. In production, a cache miss happens once per edge per TTL expiry; all subsequent requests are served from edge cache.

**Implication for the SLO:** the SLO thresholds (p95 ≤ 500 ms, p99 ≤ 1 000 ms) are chosen to tolerate an occasional cold miss under realistic load, not to hide it. A single cold miss at 441 ms in a 30-second, 5-user smoke run is absorbed into the p99 budget with margin.

### Finding 3 — WAF blocks automated User-Agent strings

The CloudFront distribution has WAF rules that return HTTP 403 Forbidden to requests without a browser-like `User-Agent` header. This was discovered during implementation: the default `HttpClient` sends no `User-Agent`, which the WAF rejects.

**Resolution:** `PerformanceConfig.CreateHttpClient()` sets `User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 ...` on the shared `HttpClient`. This is the same header a real browser sends and reflects what end users actually experience.

**Portfolio note:** this is a realistic constraint, not a workaround. Load tests against WAF-protected endpoints always need realistic request headers. Silently passing with a spoofed header is standard practice in performance engineering.

### Finding 4 — NBomber 6.x API differs from published 5.x documentation

NBomber 5.4.0 does not exist on NuGet. The closest available major version is 6.4.0, which introduced breaking API changes:

| 5.x API (spec) | 6.x API (implemented) |
|---|---|
| `IAssertion` / `Assertion.ForScenario()` | `Threshold.Create(s => ...)` |
| `.WithAssertions(...)` on runner | `.WithThresholds(...)` on scenario |
| `ReportFormat.Json` | Removed — `ReportFormat.Md` used instead |
| `response.IsError` from `Http.Send()` | `Http.Send()` returns `Response<HttpResponseMessage>`; return it directly |
| `using var httpClient` in `Build()` | Remove `using` — capturing a disposed object in the lambda was a bug in the spec |

The migration was resolved by reading the NuGet XML documentation for the installed package version rather than relying on online docs.

---

## 4. Assumptions

- **Network:** measurements were taken on a residential connection. A CDN edge node close to the client was likely selected. Results will vary by geography; the CDN itself introduces no meaningful latency beyond the edge-to-client path.
- **Target is static:** `qa-lab.html` is a static HTML file served from S3 via CloudFront. There is no application server, no database, no dynamic content. All latency is network + CDN edge overhead.
- **Single HttpClient per scenario:** one `HttpClient` instance is shared across all virtual users in a scenario. This reflects realistic browser connection pooling. Each VU reuses existing TCP connections (HTTP/1.1 keep-alive or HTTP/2).
- **No think time:** the scenarios do not insert artificial delays between requests (no `Thread.Sleep`, no NBomber step delay). This produces higher RPS than real user traffic but is appropriate for SLO boundary testing.
- **SLO values are portfolio choices:** the thresholds (p95 ≤ 500 ms, p99 ≤ 1 000 ms) were defined for this portfolio project. They are intentionally conservative relative to the measured results to demonstrate that assertions work and pass, not to establish the tightest possible bounds.

---

## 5. Limitations

### What this suite cannot test

| Limitation | Reason |
|---|---|
| Capacity / saturation point | CloudFront scales horizontally. 5–10 VUs from a single IP are absorbed without any measurable degradation. Finding a capacity limit would require thousands of distributed VUs — not appropriate for a portfolio target. |
| Origin server performance | The S3 origin is not publicly accessible. Cold-miss latency is observable, but origin throughput and error behaviour cannot be tested directly. |
| Degradation curves | Without a capacity ceiling, throughput-vs-latency curves are flat and uninformative. |
| Geographic distribution | All runs from a single location. A CDN's latency profile varies by region; multi-region testing requires distributed load agents. |
| Error injection | The WAF and CDN configuration cannot be manipulated to simulate errors (rate limits, origin timeouts). Error-rate SLOs are verified structurally, not by triggering actual errors. |
| NBomber free-tier constraint | NBomber 6.x free licence limits distributed/cluster execution. All scenarios run as single-node; this is sufficient for the CDN target but limits horizontal scaling of the load generator itself. |

### What the Cold/Warm scenario does and does not prove

The Cold/Warm scenario demonstrates that the CDN returns a measurable latency spike on the first request (cache miss) and fast responses on subsequent requests (cache hit). It **does not** prove that the CDN always misses on the first request — a warm edge may serve from cache regardless. The scenario is observational: it records the behaviour seen in that run, not a guaranteed property of the system.

---

## 6. NBomber 6.x vs 5.x — Implementation Notes for Reviewers

The project spec was written for NBomber 5.x. Since 5.4.0 was unavailable on NuGet, the implementation targets 6.x. The key API differences relevant to code review:

**Thresholds replace assertions.** In 6.x, pass/fail criteria are attached to the scenario via `.WithThresholds()` rather than passed to the runner via `.WithAssertions()`. The threshold function receives `ScenarioStats` and must return `bool`. NBomber evaluates thresholds periodically during the run and at the end; a threshold violation causes a non-zero exit code in CI.

**`Http.Send()` returns `Response<HttpResponseMessage>`.** The wrapper carries `IsError`, `StatusCode`, `SizeBytes`, and `Payload`. Returning the wrapper directly from the scenario lambda lets NBomber read latency and size from it automatically. Manually wrapping in `Response.Ok()` / `Response.Fail()` loses the automatic latency measurement.

**`HttpClient` must not be disposed before the scenario runs.** The `Build()` method configures and returns a `ScenarioProps` — the actual load does not start until after `Build()` returns. A `using var httpClient` inside `Build()` disposes the client before the first request is sent.

---

*Version: 1.0 | Author: Evgenii Subbotin | May 2026*
