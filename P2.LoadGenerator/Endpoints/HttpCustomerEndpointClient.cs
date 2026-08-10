using System.Diagnostics;
using System.Text;
using P2.LoadGenerator.Execution;

namespace P2.LoadGenerator.Endpoints;

/// <summary>
/// Sends real HTTP requests at P1's customer endpoint. Use once the storefront exists;
/// until then, use <see cref="SimulatedCustomerEndpointClient"/>.
/// </summary>
public sealed class HttpCustomerEndpointClient : ICustomerEndpointClient
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointPath;

    /// <param name="httpClient">
    /// Should be created with BaseAddress = LoadTestConfiguration.TargetBaseUrl. Injected
    /// rather than constructed here so the caller controls lifetime/pooling — one HttpClient
    /// should be reused across a whole run.
    /// </param>
    /// <param name="endpointPath">Relative path of the customer endpoint, e.g. "/api/customers".</param>
    public HttpCustomerEndpointClient(HttpClient httpClient, string endpointPath)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _endpointPath = endpointPath ?? throw new ArgumentNullException(nameof(endpointPath));
    }

    public async Task<RequestResult> SendAsync(CustomerRequest request, CancellationToken cancellationToken)
    {
        var startedAt = DateTimeOffset.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var queryParams = new Dictionary<string, string>(request.Parameters)
            {
                ["customerId"] = request.CustomerId
            };
            var queryString = BuildQueryString(queryParams);

            var uri = $"{_endpointPath}?{queryString}";
            using var response = await _httpClient.GetAsync(uri, cancellationToken);
            stopwatch.Stop();

            return new RequestResult
            {
                RequestId = request.RequestId,
                StartedAtUtc = startedAt,
                LatencyMs = stopwatch.Elapsed.TotalMilliseconds,
                Success = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode
            };
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw; // genuine cancellation should propagate, not be swallowed as a "failed" result
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            return new RequestResult
            {
                RequestId = request.RequestId,
                StartedAtUtc = startedAt,
                LatencyMs = stopwatch.Elapsed.TotalMilliseconds,
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private static string BuildQueryString(IReadOnlyDictionary<string, string> parameters)
    {
        var builder = new StringBuilder();
        var first = true;

        foreach (var (key, value) in parameters)
        {
            if (!first) builder.Append('&');
            builder.Append(Uri.EscapeDataString(key)).Append('=').Append(Uri.EscapeDataString(value));
            first = false;
        }

        return builder.ToString();
    }
}