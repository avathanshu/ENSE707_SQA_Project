using P2.LoadGenerator.Metrics;

namespace P2.LoadGenerator.Export;

/// <summary>
/// Prints a completed run's results to the terminal. This replaces the JSON export path temporarily
/// </summary>
public sealed class ConsoleResultPrinter : IResultExporter
{
    public Task<string> ExportAsync(LoadTestRunResult result, CancellationToken cancellationToken = default)
    {
        Console.WriteLine();
        Console.WriteLine($"=== Run {result.RunId} ({result.RunLabel}) ===");
        Console.WriteLine($"Profile:   {result.ProfileName}");
        Console.WriteLine($"Started:   {result.StartedAtUtc:u}");
        Console.WriteLine($"Completed: {result.CompletedAtUtc:u}");
        Console.WriteLine($"Duration:  {result.Duration}");
        Console.WriteLine();
        Console.WriteLine($"{"Endpoint",-30} {"Reqs",6} {"Fail",6} {"Avg(ms)",9} {"P50",7} {"P95",7} {"P99",7} {"Err%",6}");

        foreach (var m in result.EndpointMetrics)
        {
            Console.WriteLine(
                $"{m.EndpointName,-30} {m.TotalRequests,6} {m.FailureCount,6} " +
                $"{m.AverageLatencyMs,9:F2} {m.P50LatencyMs,7:F2} {m.P95LatencyMs,7:F2} {m.P99LatencyMs,7:F2} {m.ErrorRatePercent,6:F2}");
        }

        Console.WriteLine();

        // Return value kept for interface compatibility with IResultExporter (e.g. a
        // JSON-writing implementation) — "console" here just signals no file was written.
        return Task.FromResult("console");
    }
}
