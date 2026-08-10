// IRegressionDetector.cs
// Abstraction for "compare a new run against history and say whether
// performance regressed". Kept as an interface so the comparison strategy
// (simple threshold vs. something smarter later) can change without
// touching the dashboard or report code that consumes the results.

using System.Collections.Generic;
using P3.Core.Models;

namespace P3.Core.Regression
{
    public interface IRegressionDetector
    {
        // Compares the given run against the supplied historical runs and
        // returns one RegressionResult per endpoint present in currentRun.
        // historicalRuns should be every PREVIOUS run available (typically
        // ITestRunStore.GetAllRuns()) — the detector decides internally how
        // much of that history to use as the baseline.
        List<RegressionResult> DetectRegressions(TestRun currentRun, IReadOnlyList<TestRun> historicalRuns);
    }
}
