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

    // CDN WAF requires a browser-like User-Agent; automated UA strings return 403
    public const string UserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
        "(KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

    public static HttpClient CreateHttpClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        return client;
    }
}
