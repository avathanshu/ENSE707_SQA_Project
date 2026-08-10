// IReportExporter.cs
// Abstraction for "turn stored runs + regression results into a shareable
// QA report". Kept as an interface so we could add e.g. a PDF or HTML
// exporter later alongside the Markdown one without changing calling code.

using System.Collections.Generic;
using P3.Core.Models;

namespace P3.Core.Reporting
{
    public interface IReportExporter
    {
        // Writes a QA report covering the given runs and regression results
        // to outputPath, and returns that same path for convenience.
        string Export(IReadOnlyList<TestRun> runs, IReadOnlyList<RegressionResult> regressionResults, string outputPath);
    }
}
