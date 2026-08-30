using P2.LoadGenerator.Execution;

namespace P2.LoadGenerator.Metrics;
public sealed class RunMetricsAggregator
{
    public EndpointLoadMetric Aggregate(string endpointName, IReadOnlyList<RequestResult> results)
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
                MaxLatencyMs = 0
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
            MaxLatencyMs = latencies[^1]
        };
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
