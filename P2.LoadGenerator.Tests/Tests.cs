using P2.LoadGenerator.Configuration;
using P2.LoadGenerator.Endpoints;
using P2.LoadGenerator.Execution;
using P2.LoadGenerator.Export;
using P2.LoadGenerator.Metrics;
using P2.LoadGenerator.Profiles;

namespace P2.LoadGenerator.Tests;

[TestClass]
public class Tests
{
    // Verifies ErrorRatePercent handles zero total requests without a
    // divide-by-zero. Should return 0, not throw or return NaN.
    [TestMethod]
    public void ErrorRatePercent_ZeroTotalRequests_ReturnsZero()
    {
        var metric = new EndpointLoadMetric
        {
            EndpointName = "/api/test",
            TotalRequests = 0,
            SuccessCount = 0,
            FailureCount = 0,
            AverageLatencyMs = 0,
            P50LatencyMs = 0,
            P95LatencyMs = 0,
            P99LatencyMs = 0,
            MaxLatencyMs = 0
        };

        Assert.AreEqual(0, metric.ErrorRatePercent);
    }

    // Confirms a run with zero failures reports exactly 0% error rate.
    [TestMethod]
    public void ErrorRatePercent_AllSuccess_ReturnsZero()
    {
        var metric = new EndpointLoadMetric
        {
            EndpointName = "/api/test",
            TotalRequests = 100,
            SuccessCount = 100,
            FailureCount = 0,
            AverageLatencyMs = 40,
            P50LatencyMs = 40,
            P95LatencyMs = 60,
            P99LatencyMs = 70,
            MaxLatencyMs = 75
        };

        Assert.AreEqual(0, metric.ErrorRatePercent);
    }

    // Checks the percentage math itself is correct and rounded properly
    // for a realistic partial-failure scenario (5 failures out of 215).
    [TestMethod]
    public void ErrorRatePercent_PartialFailures_ReturnsCorrectPercentage()
    {
        var metric = new EndpointLoadMetric
        {
            EndpointName = "/api/test",
            TotalRequests = 215,
            SuccessCount = 210,
            FailureCount = 5,
            AverageLatencyMs = 40.97,
            P50LatencyMs = 39.89,
            P95LatencyMs = 67.49,
            P99LatencyMs = 75.22,
            MaxLatencyMs = 80
        };

        // 5 / 215 * 100 = 2.33 (rounded)
        Assert.AreEqual(2.33, metric.ErrorRatePercent);
    }

    // Confirms a fully-failed run reports 100%, not an off-by-something
    // value from the rounding logic.
    [TestMethod]
    public void ErrorRatePercent_AllFailures_ReturnsOneHundred()
    {
        var metric = new EndpointLoadMetric
        {
            EndpointName = "/api/test",
            TotalRequests = 50,
            SuccessCount = 0,
            FailureCount = 50,
            AverageLatencyMs = 5000,
            P50LatencyMs = 5000,
            P95LatencyMs = 5000,
            P99LatencyMs = 5000,
            MaxLatencyMs = 5000
        };

        Assert.AreEqual(100, metric.ErrorRatePercent);
    }

    // Verifies the factory is deterministic: two instances built with the
    // same seed should generate the same customer and parameters, since
    // reproducible test data matters for comparing runs.
    [TestMethod]
    public void RandomCustomerRequestFactory_SameSeed_ProducesSameCustomerAndParameters()
    {
        var factoryA = new RandomCustomerRequestFactory(seed: 42);
        var factoryB = new RandomCustomerRequestFactory(seed: 42);

        var requestA = factoryA.Create();
        var requestB = factoryB.Create();

        // RequestId uses Guid.NewGuid() regardless of seed, so it's excluded here.
        Assert.AreEqual(requestA.CustomerId, requestB.CustomerId);
        CollectionAssert.AreEqual(requestA.Parameters, requestB.Parameters);
    }

    // Sanity check that different seeds actually produce different output,
    // confirming the "randomness" isn't secretly hardcoded or ignored.
    [TestMethod]
    public void RandomCustomerRequestFactory_DifferentSeeds_LikelyProduceDifferentCustomers()
    {
        var factoryA = new RandomCustomerRequestFactory(seed: 1);
        var factoryB = new RandomCustomerRequestFactory(seed: 2);

        var requestA = factoryA.Create();
        var requestB = factoryB.Create();

        Assert.AreNotEqual(requestA.CustomerId, requestB.CustomerId);
    }

    // Proves the generator survives a run where most requests fail, rather
    // than crashing, and that the failures actually get recorded in the
    // resulting metrics instead of being silently dropped.
    [TestMethod]
    public async Task RunAsync_HighFailureRate_CompletesWithoutThrowing()
    {
        var profile = new SpikeProfile(
            duration: TimeSpan.FromSeconds(2),
            baselineConcurrency: 2,
            spikeConcurrency: 5,
            spikeStart: TimeSpan.FromSeconds(1),
            spikeDuration: TimeSpan.FromMilliseconds(500));

        var config = new LoadTestConfiguration
        {
            TargetBaseUrl = "http://localhost:5000",
            CustomerEndpointPath = "/api/customers/orders",
            Profile = profile,
            MaxConcurrency = 10,
            BatchSize = 2,
            RequestTimeout = TimeSpan.FromSeconds(1),
            RunLabel = "reliability-test-high-failure"
        };

        ICustomerEndpointClient endpointClient = new SimulatedCustomerEndpointClient(
            baseLatencyMs: 10,
            latencyJitterMs: 5,
            errorRate: 0.9,
            seed: 1);

        var requestFactory = new RandomCustomerRequestFactory(seed: 1);
        var generator = new Execution.LoadGenerator(config, endpointClient, requestFactory);

        var result = await generator.RunAsync();

        Assert.IsNotNull(result);
        Assert.AreEqual(1, result.EndpointMetrics.Count);
        Assert.IsTrue(result.EndpointMetrics[0].FailureCount > 0,
            "Expected a high-error-rate run to record at least one failure.");
    }

    // Confirms the printed console output actually contains the endpoint
    // name and table header for a normal, one-endpoint result.
    [TestMethod]
    public async Task ExportAsync_OneEndpoint_PrintsEndpointNameAndHeader()
    {
        var originalOut = Console.Out;
        var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            var result = new LoadTestRunResult
            {
                RunId = Guid.NewGuid(),
                RunLabel = "printer-test",
                ProfileName = "Spike",
                StartedAtUtc = DateTimeOffset.UtcNow,
                CompletedAtUtc = DateTimeOffset.UtcNow.AddSeconds(20),
                EndpointMetrics = new[]
                {
                    new EndpointLoadMetric
                    {
                        EndpointName = "/api/customers/orders",
                        TotalRequests = 215,
                        SuccessCount = 210,
                        FailureCount = 5,
                        AverageLatencyMs = 40.97,
                        P50LatencyMs = 39.89,
                        P95LatencyMs = 67.49,
                        P99LatencyMs = 75.22,
                        MaxLatencyMs = 80
                    }
                }
            };

            var printer = new ConsoleResultPrinter();
            await printer.ExportAsync(result);

            var output = writer.ToString();
            StringAssert.Contains(output, "Endpoint");
            StringAssert.Contains(output, "/api/customers/orders");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    // Confirms the printer doesn't throw on a run with no endpoint metrics
    // at all, and still prints the run-level info (label, profile, etc).
    [TestMethod]
    public async Task ExportAsync_NoEndpoints_PrintsHeaderWithoutThrowing()
    {
        var originalOut = Console.Out;
        var writer = new StringWriter();
        Console.SetOut(writer);

        try
        {
            var result = new LoadTestRunResult
            {
                RunId = Guid.NewGuid(),
                RunLabel = "printer-test-empty",
                ProfileName = "Spike",
                StartedAtUtc = DateTimeOffset.UtcNow,
                CompletedAtUtc = DateTimeOffset.UtcNow.AddSeconds(5),
                EndpointMetrics = Array.Empty<EndpointLoadMetric>()
            };

            var printer = new ConsoleResultPrinter();
            await printer.ExportAsync(result);

            var output = writer.ToString();
            StringAssert.Contains(output, "printer-test-empty");
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
