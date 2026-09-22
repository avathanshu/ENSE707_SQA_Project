namespace P2.LoadGenerator.Export;

/// <summary>
/// Mirrors P3.Core.Models.TestRun field-for-field (see Documentation/P3-README.md,
/// "The data contract"). Deliberately duplicated here rather than referenced from P3 —
/// P2 and P3 are still independently built/tested modules, and this is the shared JSON
/// shape they agree on, not shared code. If the contract ever drifts, a test in
/// P2.LoadGenerator.Tests comparing against this shape will catch it.
/// </summary>
public sealed class TestRunExportDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string BuildId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string LoadProfile { get; set; } = string.Empty;
    public List<EndpointMetricExportDto> EndpointMetrics { get; set; } = new();

    // Left null on export: P2 (load generation) doesn't decide this — it's filled in by
    // whichever anomaly-detection step runs after this file lands, same as the P3-README
    // documents for JsonTestRunStore-backed TestRun records.
    public bool? AnomalyDetected { get; set; }
}

public sealed class EndpointMetricExportDto
{
    public string EndpointName { get; set; } = string.Empty;
    public double LatencyP50Ms { get; set; }
    public double LatencyP95Ms { get; set; }
    public double LatencyP99Ms { get; set; }
    public double ErrorRatePercent { get; set; }
    public double ThroughputRps { get; set; }
}
