using P2.LoadGenerator.Execution;
using P2.LoadGenerator.Metrics;

namespace P2.LoadGenerator.Detection;

/// <summary>
/// Strategy pattern (mirrors <see cref="P2.LoadGenerator.Profiles.ILoadProfile"/>): decouples
/// "how do we decide something is anomalous" from the orchestrator that collects results.
/// Starts as a simple statistical/fixed-threshold baseline; a future ML-based detector could
/// implement this same interface without touching <see cref="P2.LoadGenerator.Execution.LoadGenerator"/>
/// or the exporter (Open/Closed Principle).
/// </summary>
public interface IAnomalyDetector
{
    /// <summary>
    /// Inspects the full set of results for one endpoint and returns any flags raised.
    /// Takes the whole result set (not a running stream) because several checks — e.g.
    /// percentile-based outliers, error-clustering-in-time — need the complete picture.
    /// </summary>
    IReadOnlyList<AnomalyFlag> Detect(string endpointName, IReadOnlyList<RequestResult> results);
}
