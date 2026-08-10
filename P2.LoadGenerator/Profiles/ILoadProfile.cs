namespace P2.LoadGenerator.Profiles;

/// <summary>
/// Strategy pattern: each implementation decides target concurrency at a given point
/// in the run. New profiles can be added without touching execution/metrics code
/// (Open/Closed Principle).
/// </summary>
public interface ILoadProfile
{
    /// <summary>Short identifier stored on the run result, e.g. "SteadyRamp", "Spike", "Soak".</summary>
    string Name { get; }

    /// <summary>Total planned duration of the profile.</summary>
    TimeSpan Duration { get; }

    /// <summary>Target concurrency at the given elapsed time since the run started.</summary>
    int GetTargetConcurrency(TimeSpan elapsed);
}