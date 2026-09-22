using P2.LoadGenerator.Execution;

namespace P2.LoadGenerator.Metrics;
public sealed class RunMetricsAggregator
{
    /// <param name="runDuration">Wall-clock duration of the run these results came from,
    /// used only to compute <see cref="EndpointLoadMetric.ThroughputRps"/>.</param>
    public EndpointLoadMetric Aggregate(string endpointName, IReadOnlyList<RequestResult> results, TimeSpan runDuration)
    {
        if (results.Count == 0)
        {
            return new EndpointLoadMetric
            {
                EndpointName = endpointName,
                TotalRequests = 0,
                SuccessCount = 0,
                FailureCount = 0,
                AverageLatencyMs = 0,
                P50LatencyMs = 0,
                P95LatencyMs = 0,
                P99LatencyMs = 0,
                MaxLatencyMs = 0,
                ThroughputRps = 0
            };
        }

        var latencies = results.Select(r => r.LatencyMs).OrderBy(l => l).ToList();
        var successCount = results.Count(r => r.Success);
        var failureCount = results.Count - successCount;

        return new EndpointLoadMetric
        {
            EndpointName = endpointName,
            TotalRequests = results.Count,
            SuccessCount = successCount,
            FailureCount = failureCount,
            AverageLatencyMs = latencies.Average(),
            P50LatencyMs = Percentile(latencies, 0.50),
            P95LatencyMs = Percentile(latencies, 0.95),
            P99LatencyMs = Percentile(latencies, 0.99),
            MaxLatencyMs = latencies[^1],
            ThroughputRps = ComputeThroughputRps(successCount, runDuration)
        };
    }

    private static double ComputeThroughputRps(int successCount, TimeSpan runDuration)
    {
        // Guard the same way ErrorRatePercent guards its own divide-by-zero: a run with
        // effectively no elapsed time can't sensibly report a rate.
        if (runDuration.TotalSeconds <= 0)
        {
            return 0;
        }

        return Math.Round(successCount / runDuration.TotalSeconds, 2);
    }

    private static double Percentile(List<double> sortedLatencies, double percentile)
    {
        if (sortedLatencies.Count == 1)
        {
            return sortedLatencies[0];
        }

        var index = (int)Math.Ceiling(percentile * sortedLatencies.Count) - 1;
        index = Math.Clamp(index, 0, sortedLatencies.Count - 1);
        return sortedLatencies[index];
    }
}
