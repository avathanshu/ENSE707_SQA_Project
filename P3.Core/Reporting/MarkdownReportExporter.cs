// MarkdownReportExporter.cs
// Produces the "exportable QA report" required by P3's scope. Chose Markdown
// as the format because it's plain text (easy to diff/review in Git, no
// extra libraries needed to write it), renders nicely on GitHub, and can be
// converted to PDF/HTML later with pandoc if the team wants a fancier deliverable.
//
// The report has three sections, matching the sample requirements table:
//   1. Breach log   — every Major regression (build-over-build).
//   2. Anomaly log  — every run P2's ML.NET model flagged as anomalous.
//   3. Trend chart  — a text table of p95 latency per endpoint over time,
//                      good enough to read here and also the same data the
//                      dashboard's chart is built from.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using P3.Core.Models;

namespace P3.Core.Reporting
{
    public class MarkdownReportExporter : IReportExporter
    {
        public string Export(IReadOnlyList<TestRun> runs, IReadOnlyList<RegressionResult> regressionResults, string outputPath)
        {
            // StringBuilder is used instead of string concatenation because
            // we're appending many lines — it avoids creating a new string
            // object on every single line, which matters once reports grow.
            var sb = new StringBuilder();

            sb.AppendLine("# QA Report — Regression Tracking");
            // "u" format gives a sortable, unambiguous UTC timestamp for the report header.
            sb.AppendLine($"Generated: {DateTime.UtcNow:u}");
            sb.AppendLine();

            AppendBreachLog(sb, regressionResults);
            AppendAnomalyLog(sb, runs);
            AppendTrendTable(sb, runs);

            // Ensure the destination folder exists before writing, same
            // defensive pattern used in JsonTestRunStore.
            var directory = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(outputPath, sb.ToString());
            return outputPath;
        }

        // Section 1: every Major-severity regression, i.e. the build-over-build breaches.
        private void AppendBreachLog(StringBuilder sb, IReadOnlyList<RegressionResult> regressionResults)
        {
            sb.AppendLine("## Breach Log (Build-over-Build Regressions)");

            var breaches = regressionResults
                .Where(r => r.Severity == RegressionSeverity.Major)
                .OrderByDescending(r => r.PercentChange) // worst regression first
                .ToList();

            if (breaches.Count == 0)
            {
                sb.AppendLine("No major regressions detected.");
            }
            else
            {
                sb.AppendLine("| Endpoint | Build | Baseline Build(s) | Current p95 (ms) | Baseline p95 (ms) | % Change |");
                sb.AppendLine("|---|---|---|---|---|---|");
                foreach (var r in breaches)
                {
                    // F1 = one decimal place, keeps the table compact and readable.
                    sb.AppendLine($"| {r.EndpointName} | {r.CurrentBuildId} | {r.BaselineBuildId} | {r.CurrentP95Ms:F1} | {r.BaselineP95Ms:F1} | {r.PercentChange:F1}% |");
                }
            }

            sb.AppendLine();
        }

        // Section 2: every run P2's model flagged as anomalous (AnomalyDetected == true).
        private void AppendAnomalyLog(StringBuilder sb, IReadOnlyList<TestRun> runs)
        {
            sb.AppendLine("## Anomaly Log (P2 ML.NET Flags)");

            // "== true" (rather than just checking the nullable bool truthily)
            // makes it explicit we're excluding both false AND null (not-yet-evaluated) runs.
            var anomalousRuns = runs.Where(r => r.AnomalyDetected == true).ToList();

            if (anomalousRuns.Count == 0)
            {
                sb.AppendLine("No anomalies flagged by the ML.NET detector yet.");
            }
            else
            {
                sb.AppendLine("| Build | Timestamp (UTC) | Load Profile |");
                sb.AppendLine("|---|---|---|");
                foreach (var r in anomalousRuns)
                {
                    sb.AppendLine($"| {r.BuildId} | {r.Timestamp:u} | {r.LoadProfile} |");
                }
            }

            sb.AppendLine();
        }

        // Section 3: a plain-text trend table of p95 latency per endpoint over time —
        // the same underlying data the dashboard's trend chart visualises.
        private void AppendTrendTable(StringBuilder sb, IReadOnlyList<TestRun> runs)
        {
            sb.AppendLine("## Historical Trend (p95 latency by endpoint)");

            // Flatten every run's endpoint metrics into single rows, sorted so
            // each endpoint's history reads chronologically together.
            var rows = runs
                .SelectMany(r => r.EndpointMetrics.Select(m => new { r.BuildId, r.Timestamp, m.EndpointName, m.LatencyP95Ms }))
                .OrderBy(x => x.EndpointName)
                .ThenBy(x => x.Timestamp)
                .ToList();

            if (rows.Count == 0)
            {
                sb.AppendLine("No test run data available yet.");
                return;
            }

            sb.AppendLine("| Endpoint | Build | Timestamp (UTC) | p95 (ms) |");
            sb.AppendLine("|---|---|---|---|");
            foreach (var row in rows)
            {
                sb.AppendLine($"| {row.EndpointName} | {row.BuildId} | {row.Timestamp:u} | {row.LatencyP95Ms:F1} |");
            }

            sb.AppendLine();
        }
    }
}
