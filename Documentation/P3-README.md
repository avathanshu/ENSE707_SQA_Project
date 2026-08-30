# P3 — Regression Tracking + Reporting

This covers the P3 module scope: persisting results across builds, detecting
build-over-build performance regression, a live/historical dashboard, and an
exportable QA report.

## ⚠️ Not yet compiled/run

This was written in a sandbox with no .NET SDK installed, so it has **not**
been built or run yet — only reviewed by hand for correctness. Build it
locally first thing and fix anything the compiler flags before relying on it.
Every file is heavily commented to make that review fast.

## Structure

```
P3-RegressionTracking/
├── P3.Core/                     # Class library — no UI, no external NuGet packages
│   ├── Models/                  # TestRun, EndpointMetric, RegressionResult, RegressionSeverity
│   ├── Persistence/              # ITestRunStore + JsonTestRunStore (file-based, offline)
│   ├── Regression/               # IRegressionDetector + ThresholdRegressionDetector
│   └── Reporting/                # IReportExporter + MarkdownReportExporter
├── P3.Dashboard/                # Blazor Server web app (references P3.Core)
│   ├── Pages/                    # Dashboard.razor ("/"), Trends.razor ("/trends"), _Host.cshtml
│   ├── Components/TrendChart.razor  # hand-drawn SVG line chart, no charting library
│   └── Shared/MainLayout.razor
└── SampleData/sample_runs.json  # a handful of fake runs for local dev/testing only
```

## The data contract (share this with P1 and P2)

Everything hinges on the `TestRun` shape in `P3.Core/Models/TestRun.cs`:

- `BuildId` (string) — Git SHA or CI build number. This is what regression
  detection groups/compares by.
- `Timestamp` (UTC datetime)
- `LoadProfile` (string) — "SteadyRamp" / "Spike" / "Soak"
- `EndpointMetrics` (list) — one entry per endpoint, each with
  `EndpointName`, `LatencyP50Ms`, `LatencyP95Ms`, `LatencyP99Ms`,
  `ErrorRatePercent`, `ThroughputRps`
- `AnomalyDetected` (bool?, nullable) — left `null` until P2's ML.NET model
  fills it in; `true`/`false` once evaluated.

Whoever is generating the fake/synthetic dataset should match this shape —
`SampleData/sample_runs.json` is a working example of exactly this format
and is what `JsonTestRunStore` expects to read/write.

## How build-over-build regression detection works

`ThresholdRegressionDetector` takes the newest run, looks at each endpoint,
and averages that endpoint's p95 latency across the last N prior builds
(`RegressionThresholds.BaselineWindowSize`, default 5) to form a baseline.
It then classifies the current build as:

- **None** — within `MinorPercentThreshold` (default 10%) of baseline
- **Minor** — ≥10% slower than baseline
- **Major** — ≥25% slower than baseline (this is the "breach" that shows up
  in the QA report's breach log)

This is intentionally about trend-over-time, separate from P2's job of
flagging a single run as statistically anomalous on its own.

## Running it locally

```bash
# from P3-RegressionTracking/
dotnet new sln -n P3RegressionTracking
dotnet sln add P3.Core/P3.Core.csproj P3.Dashboard/P3.Dashboard.csproj
dotnet build

# copy the sample data into the path Program.cs points at, or just run —
# JsonTestRunStore creates an empty file automatically if none exists yet
cp SampleData/sample_runs.json SampleData/runs.json

cd P3.Dashboard
dotnet run
```

Then open the URL `dotnet run` prints (usually `https://localhost:5001`) —
"/" is the live dashboard, "/trends" is the historical trend view.

## Next steps once P1/P2 land real data

1. Point `JsonTestRunStore`'s file path (in `Program.cs`) at wherever
   P1's load generator actually writes results, or add a small adapter that
   converts P1's raw output into `TestRun` objects and calls `SaveRun`.
2. Have P2's ML.NET pipeline set `AnomalyDetected` on each `TestRun` before
   (or after) it's saved.
3. If the JSON file approach gets unwieldy, swap `JsonTestRunStore` for a
   SQLite-backed implementation of `ITestRunStore` — nothing else in the
   app needs to change, since everything depends on the interface.
