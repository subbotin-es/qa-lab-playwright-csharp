using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using QALab.Performance.Config;

namespace QALab.Performance.Scenarios;

public static class SmokeScenario
{
    public static ScenarioProps Build()
    {
        var httpClient = PerformanceConfig.CreateHttpClient();

        return Scenario.Create("qa_lab_smoke", async context =>
        {
            var request = Http.CreateRequest("GET", $"{PerformanceConfig.BaseUrl}{PerformanceConfig.QALabPath}");
            return await Http.Send(httpClient, request);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingConstant(copies: 5, during: TimeSpan.FromSeconds(10)),
            Simulation.KeepConstant(copies: 5, during: TimeSpan.FromSeconds(20))
        )
        .WithThresholds(
            Threshold.Create(s => s.Fail.Request.Percent < 1.0),
            Threshold.Create(s => s.Ok.Latency.Percent95 <= PerformanceConfig.P95ThresholdMs),
            Threshold.Create(s => s.Ok.Latency.Percent99 <= PerformanceConfig.P99ThresholdMs)
        );
    }
}
