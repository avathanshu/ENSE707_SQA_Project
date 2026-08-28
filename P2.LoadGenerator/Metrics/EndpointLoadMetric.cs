namespace P2.LoadGenerator.Metrics;

/// <summary>
/// Aggregated statistics for one endpoint over the course of a run. This is P2's own
/// internal metric shape.
/// </summary>
public sealed record EndpointLoadMetric
{
    public required string EndpointName { get; init; }
    public required int TotalRequests { get; init; }
    public required int SuccessCount { get; init; }
    public required int FailureCount { get; init; }
    public required double AverageLatencyMs { get; init; }
    public required double P50LatencyMs { get; init; }
    public required double P95LatencyMs { get; init; }
    public required double P99LatencyMs { get; init; }
    public required double MaxLatencyMs { get; init; }

    public double ErrorRatePercent => TotalRequests == 0
        ? 0
        : Math.Round(FailureCount / (double)TotalRequests * 100, 2);
}
