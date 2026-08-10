// RegressionThresholds.cs
// Holds the tunable numbers the regression detector uses to decide severity.
// Pulling these into one class (instead of hardcoding numbers inside the
// detector) makes them easy to justify/cite in the report's requirements
// section, and easy to adjust during testing without touching detector logic.

namespace P3.Core.Regression
{
    public class RegressionThresholds
    {
        // If the current build's p95 latency is at least this many percent
        // slower than the baseline, classify it as a Minor regression.
        // Default: 10% slower triggers a "worth watching" flag.
        public double MinorPercentThreshold { get; set; } = 10.0;

        // If the current build's p95 latency is at least this many percent
        // slower than the baseline, classify it as a Major regression
        // (a breach that belongs in the QA report's breach log).
        // Default: 25% slower is treated as a real regression.
        public double MajorPercentThreshold { get; set; } = 25.0;

        // How many of the most recent prior builds to average together to
        // form the "baseline" a new build is compared against. Using a
        // rolling average (rather than just the single previous build)
        // smooths out normal run-to-run noise so one unlucky run doesn't
        // get flagged as a false regression.
        // Default: average the last 5 builds.
        public int BaselineWindowSize { get; set; } = 5;
    }
}
