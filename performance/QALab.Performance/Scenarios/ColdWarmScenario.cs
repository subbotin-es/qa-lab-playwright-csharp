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
        var httpClient = PerformanceConfig.CreateHttpClient();
        var target = $"{PerformanceConfig.BaseUrl}{PerformanceConfig.QALabPath}";

        return Scenario.Create("cdn_cold_warm", async context =>
        {
            var coldResponse = await Http.Send(httpClient, Http.CreateRequest("GET", target));
            if (coldResponse.IsError) return Response.Fail();

            return await Http.Send(httpClient, Http.CreateRequest("GET", target));
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.IterationsForConstant(copies: 1, iterations: 10)
        );
    }
}
