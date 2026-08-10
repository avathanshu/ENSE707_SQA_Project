// RegressionResult.cs
// The output of a single build-over-build comparison for one endpoint.
// The regression detector produces a list of these; the dashboard renders
// them as the trend/breach view, and the report exporter writes them into
// the QA report's breach log.

namespace P3.Core.Models
{
    public class RegressionResult
    {
        // Which endpoint this comparison is about, e.g. "POST /checkout".
        public string EndpointName { get; set; } = string.Empty;

        // The build that was just tested (the one being evaluated).
        public string CurrentBuildId { get; set; } = string.Empty;

        // The build (or averaged window of prior builds) it was compared against.
        public string BaselineBuildId { get; set; } = string.Empty;

        // p95 latency, in milliseconds, of the current build being evaluated.
        public double CurrentP95Ms { get; set; }

        // p95 latency, in milliseconds, of the baseline it's compared to.
        public double BaselineP95Ms { get; set; }

        // How much slower (positive) or faster (negative) the current build is,
        // expressed as a percentage of the baseline:
        // (CurrentP95Ms - BaselineP95Ms) / BaselineP95Ms * 100.
        public double PercentChange { get; set; }

        // The classification of this change — None / Minor / Major —
        // decided by comparing PercentChange against configured thresholds.
        public RegressionSeverity Severity { get; set; }

        // A ready-to-display sentence explaining the result, e.g.
        // "POST /checkout is 32.4% slower than the 5-build baseline (breach)."
        // Generated once by the detector so the UI/report don't duplicate logic.
        public string Explanation { get; set; } = string.Empty;
    }
}
