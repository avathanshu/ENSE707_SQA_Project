namespace P2.LoadGenerator.Execution;

/// <summary>
/// Outcome of sending one <see cref="CustomerRequest"/> to the customer endpoint.
/// </summary>
public sealed record RequestResult
{
    public required Guid RequestId { get; init; }
    public required DateTimeOffset StartedAtUtc { get; init; }
    public required double LatencyMs { get; init; }
    public required bool Success { get; init; }
    public int? StatusCode { get; init; }
    public string? ErrorMessage { get; init; }
}
