using P2.LoadGenerator.Configuration;
using P2.LoadGenerator.Detection;
using P2.LoadGenerator.Endpoints;
using P2.LoadGenerator.Execution;
using P2.LoadGenerator.Export;
using P2.LoadGenerator.Metrics;
using P2.LoadGenerator.Profiles;

// Demo run against the simulated endpoint, since P1's storefront isn't up yet.
// Once it is: replace SimulatedCustomerEndpointClient with HttpCustomerEndpointClient
// (see Endpoints/HttpCustomerEndpointClient.cs) — nothing else below needs to change.

var profile = new SpikeProfile(
    duration: TimeSpan.FromSeconds(20),
    baselineConcurrency: 5,
    spikeConcurrency: 60,
    spikeStart: TimeSpan.FromSeconds(8),
    spikeDuration: TimeSpan.FromSeconds(4));

var config = new LoadTestConfiguration
{
    TargetBaseUrl = "http://localhost:5000",
    CustomerEndpointPath = "/api/customers/orders",
    Profile = profile,
    MaxConcurrency = 100,
    BatchSize = 10,
    RequestTimeout = TimeSpan.FromSeconds(5),
    RunLabel = "demo-spike-run"
};

ICustomerEndpointClient endpointClient = new SimulatedCustomerEndpointClient(
    baseLatencyMs: 40,
    latencyJitterMs: 15,
    errorRate: 0.02,
    seed: 42);

var requestFactory = new RandomCustomerRequestFactory(seed: 42);
var anomalyDetector = new StatisticalAnomalyDetector(new AnomalyDetectionOptions
{
    LatencyZScoreThreshold = 3.0,
    AbsoluteLatencyCeilingMs = 300, // low on purpose for the simulated client, so the spike trips it
    ErrorBurstRateThreshold = 0.25,
    ErrorBurstWindowSize = 20
});

var loadGenerator = new LoadGenerator(config, endpointClient, requestFactory, anomalyDetector: anomalyDetector);

Console.WriteLine($"Starting load test '{config.RunLabel}' with profile '{profile.Name}'...");
var result = await loadGenerator.RunAsync();

Console.WriteLine();
Console.WriteLine($"Run {result.RunId} complete — duration {result.Duration.TotalSeconds:F1}s");
foreach (var metric in result.EndpointMetrics)
{
    Console.WriteLine($"  {metric.EndpointName}: {metric.TotalRequests} requests, " +
                       $"{metric.ErrorRatePercent}% errors, avg {metric.AverageLatencyMs}ms, " +
                       $"p95 {metric.P95LatencyMs}ms, p99 {metric.P99LatencyMs}ms");
}

Console.WriteLine($"  Anomalies flagged: {result.AnomalyFlags.Count}");
foreach (var flag in result.AnomalyFlags.Take(5))
{
    Console.WriteLine($"    [{flag.Severity}] {flag.Description}");
}

var debugExporter = new JsonResultExporter(outputDirectory: "Output");
var debugPath = await debugExporter.ExportAsync(result);
Console.WriteLine();
Console.WriteLine($"P2 debug export written to: {debugPath}");

var handoffExporter = new P3HandoffExporter(outputPath: Path.Combine("Output", "p3-handoff.json"));
var handoffPath = await handoffExporter.ExportAsync(result);
Console.WriteLine($"P3 handoff export written to: {handoffPath}");
