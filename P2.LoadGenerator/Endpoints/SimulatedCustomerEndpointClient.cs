using System.Diagnostics;
using P2.LoadGenerator.Execution;

namespace P2.LoadGenerator.Endpoints;

/// <summary>
/// Stand-in for the real storefront while P1 (storefront + dataset) is still being built.
/// Simulates realistic latency and an occasional error. Swap for
/// <see cref="HttpCustomerEndpointClient"/> with no other code changes once P1 ships.
/// </summary>
public sealed class SimulatedCustomerEndpointClient : ICustomerEndpointClient
{
    private readonly Random _random;
    private readonly double _baseLatencyMs;
    private readonly double _latencyJitterMs;
    private readonly double _errorRate;

    /// <param name="baseLatencyMs">Typical latency under light load.</param>
    /// <param name="latencyJitterMs">Random variation added/subtracted from the base latency.</param>
    /// <param name="errorRate">Probability (0.0-1.0) that a given request simulates a failure.</param>
    /// <param name="seed">Optional seed for reproducible test runs.</param>
    public SimulatedCustomerEndpointClient(
        double baseLatencyMs = 40,
        double latencyJitterMs = 15,
        double errorRate = 0.02,
        int? seed = null)
    {
        _baseLatencyMs = baseLatencyMs;
        _latencyJitterMs = latencyJitterMs;
        _errorRate = errorRate;
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    public async Task<RequestResult> SendAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        var simulatedLatencyMs = Math.Max(1, _baseLatencyMs + (NextGaussianJitter() * _latencyJitterMs));
        await Task.Delay(TimeSpan.FromMilliseconds(simulatedLatencyMs), cancellationToken);
        stopwatch.Stop();

        var isError = _random.NextDouble() < _errorRate;

        return new RequestResult
        {
            RequestId = request.RequestId,
            StartedAtUtc = startedAt,
            LatencyMs = stopwatch.Elapsed.TotalMilliseconds,
            Success = !isError,
            StatusCode = isError ? 500 : 200,
            ErrorMessage = isError ? "Simulated downstream failure" : null
        };
    }

    /// <summary>Approximate standard-normal sample (Box-Muller), so simulated latency clusters
    /// around the base value instead of being uniformly noisy.</summary>
    private double NextGaussianJitter()
    {
        var u1 = 1.0 - _random.NextDouble();
        var u2 = _random.NextDouble();
        return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
    }
}