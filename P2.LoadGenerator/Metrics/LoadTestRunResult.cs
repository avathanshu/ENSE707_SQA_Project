namespace P2.LoadGenerator.Metrics;

/// <summary>
/// Everything produced by one load test run: metadata and per-endpoint metrics.
/// </summary>
public sealed record LoadTestRunResult
{
    public required Guid RunId { get; init; }
    public required string RunLabel { get; init; }
    public required string ProfileName { get; init; }
    public required DateTimeOffset StartedAtUtc { get; init; }
    public required DateTimeOffset CompletedAtUtc { get; init; }
    public required IReadOnlyList<EndpointLoadMetric> EndpointMetrics { get; init; }

    public TimeSpan Duration => CompletedAtUtc - StartedAtUtc;
}
