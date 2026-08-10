// RegressionSeverity.cs
// Classifies how bad a build-over-build performance change is, so the
// dashboard and report can colour-code / filter results without every
// caller re-implementing the same threshold logic.

namespace P3.Core.Models
{
    public enum RegressionSeverity
    {
        // Current build performs the same as, or better than, the baseline.
        None,

        // Current build is slower than baseline, but not by enough to count
        // as a hard breach — worth showing on the trend view as a warning.
        Minor,

        // Current build is slower than baseline by more than the configured
        // "major" threshold — this is a real regression breach and should
        // appear in the QA report's breach log.
        Major
    }
}
