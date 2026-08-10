using P2.LoadGenerator.Execution;
using P2.LoadGenerator.Metrics;

namespace P2.LoadGenerator.Detection;

/// <summary>
/// Baseline anomaly detector combining two checks, deliberately kept simple and explainable
/// so its output is trustworthy as a comparison point for any later ML-based detector:
///
/// 1. Latency outliers — flags individual requests whose latency z-score exceeds
///    <see cref="AnomalyDetectionOptions.LatencyZScoreThreshold"/>, or that breach the
///    absolute ceiling outright (catches uniform degradation a z-score can miss, since
///    z-score is relative to the run's own mean).
/// 2. Error bursts — flags rolling windows of requests (in chronological order) whose
///    error rate exceeds <see cref="AnomalyDetectionOptions.ErrorBurstRateThreshold"/>,
///    which surfaces clustered failures even when the overall run error rate looks fine.
/// </summary>
public sealed class StatisticalAnomalyDetector : IAnomalyDetector
{
    private readonly AnomalyDetectionOptions _options;

    public StatisticalAnomalyDetector(AnomalyDetectionOptions? options = null)
    {
        _options = options ?? AnomalyDetectionOptions.Default;
    }

    public IReadOnlyList<AnomalyFlag> Detect(string endpointName, IReadOnlyList<RequestResult> results)
    {
        var flags = new List<AnomalyFlag>();
        if (results.Count == 0) return flags;

        var ordered = results.OrderBy(r => r.StartedAtUtc).ToList();

        flags.AddRange(DetectLatencyOutliers(endpointName, ordered));
        flags.AddRange(DetectErrorBursts(endpointName, ordered));

        return flags;
    }

    private IEnumerable<AnomalyFlag> DetectLatencyOutliers(string endpointName, IReadOnlyList<RequestResult> ordered)
    {
        var latencies = ordered.Select(r => r.LatencyMs).ToList();
        var mean = latencies.Average();
        var stdDev = StandardDeviation(latencies, mean);

        foreach (var result in ordered)
        {
            var breachesCeiling = result.LatencyMs >= _options.AbsoluteLatencyCeilingMs;
            var zScore = stdDev > 0 ? (result.LatencyMs - mean) / stdDev : 0;
            var breachesZScore = Math.Abs(zScore) >= _options.LatencyZScoreThreshold;

            if (!breachesCeiling && !breachesZScore) continue;

            yield return new AnomalyFlag
            {
                EndpointName = endpointName,
                Severity = breachesCeiling ? AnomalySeverity.Critical : AnomalySeverity.Warning,
                Description = breachesCeiling
                    ? $"Request {result.RequestId} latency {result.LatencyMs:F0}ms breached the absolute ceiling of {_options.AbsoluteLatencyCeilingMs:F0}ms."
                    : $"Request {result.RequestId} latency {result.LatencyMs:F0}ms is a statistical outlier (z-score {zScore:F2}) against a run mean of {mean:F0}ms.",
                DetectedAtUtc = result.StartedAtUtc,
                ObservedValue = result.LatencyMs,
                ThresholdValue = breachesCeiling ? _options.AbsoluteLatencyCeilingMs : mean + (_options.LatencyZScoreThreshold * stdDev)
            };
        }
    }

    private IEnumerable<AnomalyFlag> DetectErrorBursts(string endpointName, IReadOnlyList<RequestResult> ordered)
    {
        var windowSize = Math.Min(_options.ErrorBurstWindowSize, ordered.Count);
        if (windowSize == 0) yield break;

        for (var start = 0; start <= ordered.Count - windowSize; start++)
        {
            var window = ordered.Skip(start).Take(windowSize).ToList();
            var errorRate = window.Count(r => !r.Success) / (double)window.Count;

            if (errorRate < _options.ErrorBurstRateThreshold) continue;

            yield return new AnomalyFlag
            {
                EndpointName = endpointName,
                Severity = AnomalySeverity.Critical,
                Description = $"Error burst detected: {errorRate:P0} of a {windowSize}-request window failed, " +
                               $"starting at request {window[0].RequestId}.",
                DetectedAtUtc = window[0].StartedAtUtc,
                ObservedValue = errorRate,
                ThresholdValue = _options.ErrorBurstRateThreshold
            };

            // Skip past this window so one sustained burst doesn't produce one flag per
            // overlapping window — the next flag will only fire once the burst clears or moves.
            start += windowSize - 1;
        }
    }

    private static double StandardDeviation(IReadOnlyList<double> values, double mean)
    {
        if (values.Count < 2) return 0;
        var sumOfSquares = values.Sum(v => Math.Pow(v - mean, 2));
        return Math.Sqrt(sumOfSquares / (values.Count - 1));
    }
}
