using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;
using QALab.Performance.Config;

namespace QALab.Performance.Scenarios;

public static class BaselineScenario
{
    public static ScenarioProps Build()
    {
        var httpClient = PerformanceConfig.CreateHttpClient();

        return Scenario.Create("qa_lab_baseline", async context =>
        {
            var path = context.InvocationNumber % 2 == 0
                ? PerformanceConfig.QALabPath
                : PerformanceConfig.QALabIndexPath;

            var request = Http.CreateRequest("GET", $"{PerformanceConfig.BaseUrl}{path}");
            return await Http.Send(httpClient, request);
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.RampingConstant(copies: 10, during: TimeSpan.FromSeconds(15)),
            Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(45))
        )
        .WithThresholds(
            Threshold.Create(s => s.Fail.Request.Percent < 1.0),
            Threshold.Create(s => s.Ok.Latency.Percent95 <= PerformanceConfig.P95ThresholdMs)
        );
    }
}
