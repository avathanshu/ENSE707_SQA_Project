namespace P2.LoadGenerator.Execution;
public sealed record CustomerRequest
{
    public required Guid RequestId { get; init; }
    public required string CustomerId { get; init; }
    public required Dictionary<string, string> Parameters { get; init; }
}
