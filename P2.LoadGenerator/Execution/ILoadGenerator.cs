using P2.LoadGenerator.Metrics;

namespace P2.LoadGenerator.Execution;

/// <summary>Runs a configured load test to completion and returns the aggregated result.</summary>
public interface ILoadGenerator
{
    Task<LoadTestRunResult> RunAsync(CancellationToken cancellationToken = default);
}
