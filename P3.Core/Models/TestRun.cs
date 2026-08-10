// TestRun.cs
// Represents ONE execution of the load test suite against ONE build.
// This is the top-level record that gets persisted by P3's storage layer,
// and is also the unit that P1 (load generator) produces and P2 (ML.NET
// anomaly detector) annotates with AnomalyDetected before it reaches P3.

using System;
using System.Collections.Generic;

namespace P3.Core.Models
{
    public class TestRun
    {
        // Unique ID for this run, generated automatically when the object is created.
        // Guid.NewGuid() creates a new random identifier so runs never collide.
        public Guid Id { get; set; } = Guid.NewGuid();

        // Identifier of the build/commit this run was executed against.
        // e.g. a short Git SHA ("a1b2c3d") or a CI build number ("build-142").
        // This is the field regression detection groups and compares runs by.
        public string BuildId { get; set; } = string.Empty;

        // When this run was executed, in UTC so comparisons aren't affected
        // by daylight saving or local time zone differences between machines.
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Which load profile P1's generator used for this run:
        // "SteadyRamp", "Spike", or "Soak" (endurance). Kept as a free string
        // so P1 can add new profiles without P3 needing a code change.
        public string LoadProfile { get; set; } = string.Empty;

        // The per-endpoint metrics captured during this run.
        // new() creates an empty list by default so callers can .Add() to it
        // without needing to null-check first.
        public List<EndpointMetric> EndpointMetrics { get; set; } = new();

        // Whether P2's ML.NET model flagged this run as anomalous.
        // Nullable (bool?) because this run may exist in storage BEFORE P2's
        // model has processed it — null means "not yet evaluated by P2",
        // as opposed to false, which means "evaluated and found normal".
        public bool? AnomalyDetected { get; set; }
    }
}
