using P2.LoadGenerator.Metrics;

namespace P2.LoadGenerator.Export;

/// <summary>
/// Writes a completed <see cref="LoadTestRunResult"/> somewhere durable. Kept as an
/// interface so a future exporter (e.g. straight into a shared database, or directly
/// calling the regression tracker's store) can be added without touching the run logic.
/// Also lets tests substitute a fake exporter without touching the console/disk.
/// </summary>
public interface IResultExporter
{
    Task<string> ExportAsync(LoadTestRunResult result, CancellationToken cancellationToken = default);
}
