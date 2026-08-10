using P2.LoadGenerator.Profiles;

namespace P2.LoadGenerator.Configuration;

/// <summary>
/// Immutable configuration for a single load test run.
/// </summary>
public sealed class LoadTestConfiguration
{
    /// <summary>Base address of the customer endpoint under test (P1's storefront API).</summary>
    public required string TargetBaseUrl { get; init; }

    /// <summary>Relative path of the customer endpoint being stress-tested.</summary>
    public required string CustomerEndpointPath { get; init; }

    /// <summary>The ramp strategy that decides concurrency-over-time (Strategy pattern).</summary>
    public required ILoadProfile Profile { get; init; }

    /// <summary>Upper bound on concurrent in-flight requests, regardless of what the profile requests.</summary>
    public int MaxConcurrency { get; init; } = 100;

    /// <summary>How many synthetic customer requests to generate per batch/time-slice.</summary>
    public int BatchSize { get; init; } = 10;

    /// <summary>Per-request timeout.</summary>
    public TimeSpan RequestTimeout { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>Human-readable label so runs are identifiable in the regression tracker's history.</summary>
    public string RunLabel { get; init; } = "unlabeled-run";
}