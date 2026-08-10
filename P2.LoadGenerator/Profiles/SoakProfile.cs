namespace P2.LoadGenerator.Profiles;

/// <summary>
/// Holds a constant, moderate concurrency for an extended duration. Models
/// endurance/soak testing — surfaces slow degradation (memory leaks, connection-pool
/// exhaustion) that only shows up under sustained load.
/// </summary>
public sealed class SoakProfile : ILoadProfile
{
    public string Name => "Soak";
    public TimeSpan Duration { get; }
    public int Concurrency { get; }

    public SoakProfile(TimeSpan duration, int concurrency)
    {
        Duration = duration;
        Concurrency = concurrency;
    }

    public int GetTargetConcurrency(TimeSpan elapsed) => Concurrency;
}