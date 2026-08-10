using P2.LoadGenerator.Execution;

namespace P2.LoadGenerator.Endpoints;

/// <summary>
/// Abstraction over "however the customer endpoint is actually reached". Lets the load
/// generator be built and tested before P1's storefront exists (Dependency Inversion).
/// </summary>
public interface ICustomerEndpointClient
{
    /// <summary>
    /// Sends a single customer request and returns its measured outcome. Implementations
    /// must not throw for ordinary failures (timeouts, non-2xx) — report via
    /// RequestResult.Success. Only genuine cancellation should propagate.
    /// </summary>
    Task<RequestResult> SendAsync(CustomerRequest request, CancellationToken cancellationToken);
}