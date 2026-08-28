namespace P2.LoadGenerator.Execution;

/// <summary>
/// RECONSTRUCTED — this file was lost before I ever saw its original content. Shape
/// inferred from CustomerRequestFactory.cs (which sets RequestId, CustomerId, and
/// Parameters) and how RequestResult correlates back to a request by RequestId.
///
/// If HttpCustomerEndpointClient.cs or SimulatedCustomerEndpointClient.cs (which I
/// also haven't seen) reference any other property on this type, the compiler will
/// tell you exactly what's missing when you build — add it back in based on the
/// error, don't guess.
/// </summary>
public sealed record CustomerRequest
{
    public required Guid RequestId { get; init; }
    public required string CustomerId { get; init; }
    public required Dictionary<string, string> Parameters { get; init; }
}
