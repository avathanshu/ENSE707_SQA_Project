using P2.LoadGenerator.Catalog;
using P2.LoadGenerator.Configuration;
using P2.LoadGenerator.Endpoints;
using P2.LoadGenerator.Execution;
using P2.LoadGenerator.Export;
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

// Uses P1's real product catalogue shape (see Catalog/ProductCatalogLoader) instead of
// the fake five-SKU list, now that P1's data contract is known.
var requestFactory = new CatalogCustomerRequestFactory(seed: 42);

var loadGenerator = new LoadGenerator(config, endpointClient, requestFactory);

Console.WriteLine($"Starting load test '{config.RunLabel}' with profile '{profile.Name}'...");
var result = await loadGenerator.RunAsync();

// Console summary for a human watching the run...
IResultExporter printer = new ConsoleResultPrinter();
await printer.ExportAsync(result);

// ...and the automated handoff to P3: writes/append this run to the same JSON shape
// P3's JsonTestRunStore reads, retiring the old manual console-copy step.
IResultExporter jsonExporter = new JsonRunExporter(
    filePath: Path.Combine(AppContext.BaseDirectory, "runs.json"),
    buildId: Environment.GetEnvironmentVariable("BUILD_ID"));
var writtenPath = await jsonExporter.ExportAsync(result);
Console.WriteLine($"Run exported to P3-compatible JSON at: {writtenPath}");
