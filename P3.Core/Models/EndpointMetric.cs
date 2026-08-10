// EndpointMetric.cs
// Holds the performance numbers collected for ONE endpoint (e.g. "POST /cart")
// during ONE load-test run. A TestRun contains a list of these — one per
// endpoint that was exercised in that run.

namespace P3.Core.Models
{
    // Plain data class (a "DTO") — no logic, just fields describing what was measured.
    public class EndpointMetric
    {
        // The endpoint that was tested, e.g. "GET /checkout" or "POST /inventory".
        // Stored as a string so P1's load generator can name endpoints freely.
        public string EndpointName { get; set; } = string.Empty;

        // Median (50th percentile) response time in milliseconds.
        // Half of all requests were faster than this, half were slower.
        public double LatencyP50Ms { get; set; }

        // 95th percentile response time in milliseconds.
        // This is the number most SLAs are written against (e.g. "p95 < 300ms").
        public double LatencyP95Ms { get; set; }

        // 99th percentile response time in milliseconds.
        // Captures worst-case / tail latency behaviour, useful for spotting
        // rare but severe slowdowns that the median or p95 might hide.
        public double LatencyP99Ms { get; set; }

        // Percentage of requests to this endpoint that failed
        // (non-2xx status code, timeout, or thrown exception).
        public double ErrorRatePercent { get; set; }

        // Successful requests per second the endpoint sustained during the run.
        // Lower throughput at the same load level can itself indicate degradation.
        public double ThroughputRps { get; set; }
    }
}
