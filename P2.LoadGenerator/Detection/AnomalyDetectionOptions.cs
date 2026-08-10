namespace P2.LoadGenerator.Detection;

/// <summary>
/// Tunable thresholds for <see cref="StatisticalAnomalyDetector"/>. Pulled out into its own
/// type (rather than constructor parameters) so it can be serialised, logged alongside a run,
/// and adjusted without changing the detector's code — the detector should never need editing
/// just to retune sensitivity.
/// </summary>
public sealed class AnomalyDetectionOptions
{
    /// <summary>Latency z-score beyond which an individual request is flagged as an outlier.</summary>
    public double LatencyZScoreThreshold { get; init; } = 3.0;

    /// <summary>Hard latency ceiling (ms) — always flagged regardless of z-score, as a fixed-threshold
    /// safety net for when a whole run degrades uniformly (z-score alone would miss that).</summary>
    public double AbsoluteLatencyCeilingMs { get; init; } = 2000;

    /// <summary>Error rate (0.0-1.0) within a rolling window above which a burst is flagged.</summary>
    public double ErrorBurstRateThreshold { get; init; } = 0.25;

    /// <summary>Size of the rolling window (by request count, in start-time order) used to detect error bursts.</summary>
    public int ErrorBurstWindowSize { get; init; } = 20;

    public static AnomalyDetectionOptions Default => new();
}
