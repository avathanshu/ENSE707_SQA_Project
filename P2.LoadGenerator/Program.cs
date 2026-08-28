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

var requestFactory = new RandomCustomerRequestFactory(seed: 42);

var loadGenerator = new LoadGenerator(config, endpointClient, requestFactory);

Console.WriteLine($"Starting load test '{config.RunLabel}' with profile '{profile.Name}'...");
var result = await loadGenerator.RunAsync();

// Prints a formatted summary table to the terminal. Swap this for a JSON exporter
// once the P3 handoff shape (BuildId source, ThroughputRps calculation) is settled.
IResultExporter printer = new ConsoleResultPrinter();
await printer.ExportAsync(result);
