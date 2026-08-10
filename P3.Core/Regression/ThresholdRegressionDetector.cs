// ThresholdRegressionDetector.cs
// Concrete IRegressionDetector implementation. For each endpoint in the
// current run, it builds a "baseline" from the last N prior builds' p95
// latency for that same endpoint, then flags the current build as
// None/Minor/Major depending on how much slower it is than that baseline.
//
// This is deliberately about BUILD-OVER-BUILD trend regression (comparing
// today's build to recent history), which is what distinguishes P3 from
// P2's job — P2's ML.NET model flags WITHIN-RUN anomalies (does this run
// look statistically weird on its own), P3 answers "is this build trending
// worse than its recent predecessors".

using System;
using System.Collections.Generic;
using System.Linq;
using P3.Core.Models;

namespace P3.Core.Regression
{
    public class ThresholdRegressionDetector : IRegressionDetector
    {
        // The threshold configuration this detector was built with.
        private readonly RegressionThresholds _thresholds;

        // Allow the caller to supply custom thresholds; fall back to the
        // class's defaults (RegressionThresholds's own default property
        // values) if none are supplied.
        public ThresholdRegressionDetector(RegressionThresholds? thresholds = null)
        {
            _thresholds = thresholds ?? new RegressionThresholds();
        }

        public List<RegressionResult> DetectRegressions(TestRun currentRun, IReadOnlyList<TestRun> historicalRuns)
        {
            var results = new List<RegressionResult>();

            // Only look at runs strictly BEFORE the current one, and never
            // compare the current run against itself if it's already been
            // saved to the store before this method runs.
            var priorRuns = historicalRuns
                .Where(r => r.Id != currentRun.Id && r.Timestamp < currentRun.Timestamp)
                .OrderByDescending(r => r.Timestamp) // most recent prior run first
                .ToList();

            // Evaluate each endpoint that was measured in the current run.
            foreach (var currentMetric in currentRun.EndpointMetrics)
            {
                // Gather this same endpoint's p95 latency from the most recent
                // prior runs, up to BaselineWindowSize of them, to average
                // into a single baseline value.
                var priorP95Values = priorRuns
                    .SelectMany(r => r.EndpointMetrics)
                    .Where(m => string.Equals(m.EndpointName, currentMetric.EndpointName, StringComparison.OrdinalIgnoreCase))
                    .Take(_thresholds.BaselineWindowSize)
                    .Select(m => m.LatencyP95Ms)
                    .ToList();

                // If there's no history yet for this endpoint, there's nothing
                // to compare against — skip it rather than reporting a false
                // 0%/undefined regression. This is expected for the very first
                // run of a brand-new endpoint.
                if (priorP95Values.Count == 0)
                {
                    continue;
                }

                var baselineP95 = priorP95Values.Average();

                // Guard against division by zero if a baseline of exactly 0ms
                // was ever recorded (shouldn't happen in practice, but a
                // defensive check is cheap and avoids a crash/NaN result).
                if (baselineP95 <= 0)
                {
                    continue;
                }

                // Percentage change: positive means slower (worse), negative
                // means faster (better) than baseline.
                var percentChange = (currentMetric.LatencyP95Ms - baselineP95) / baselineP95 * 100.0;

                var severity = ClassifySeverity(percentChange);

                results.Add(new RegressionResult
                {
                    EndpointName = currentMetric.EndpointName,
                    CurrentBuildId = currentRun.BuildId,
                    // Record which builds fed the baseline for traceability —
                    // join their IDs so the report can show exactly what was compared.
                    BaselineBuildId = string.Join(",", priorRuns
                        .Take(_thresholds.BaselineWindowSize)
                        .Select(r => r.BuildId)
                        .Distinct()),
                    CurrentP95Ms = currentMetric.LatencyP95Ms,
                    BaselineP95Ms = baselineP95,
                    PercentChange = percentChange,
                    Severity = severity,
                    Explanation = BuildExplanation(currentMetric.EndpointName, percentChange, severity)
                });
            }

            return results;
        }

        // Turns a percentage change into a severity level using the
        // configured thresholds. Only positive (slower) changes can be
        // Minor/Major — a build that got FASTER is never a "regression".
        private RegressionSeverity ClassifySeverity(double percentChange)
        {
            if (percentChange >= _thresholds.MajorPercentThreshold)
            {
                return RegressionSeverity.Major;
            }

            if (percentChange >= _thresholds.MinorPercentThreshold)
            {
                return RegressionSeverity.Minor;
            }

            return RegressionSeverity.None;
        }

        // Builds a human-readable one-line explanation for the dashboard/report.
        // "F1" formats the number to 1 decimal place, e.g. 32.4.
        private string BuildExplanation(string endpointName, double percentChange, RegressionSeverity severity)
        {
            if (severity == RegressionSeverity.None)
            {
                return $"{endpointName} is within {percentChange:F1}% of its baseline — no regression.";
            }

            var breachLabel = severity == RegressionSeverity.Major ? "breach" : "warning";
            return $"{endpointName} is {percentChange:F1}% slower than its rolling baseline ({breachLabel}).";
        }
    }
}
