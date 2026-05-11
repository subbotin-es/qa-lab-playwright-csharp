using NBomber.Contracts.Stats;
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
    .WithReportFormats(ReportFormat.Html, ReportFormat.Md)
    .WithReportFolder("performance-results")
    .Run();
