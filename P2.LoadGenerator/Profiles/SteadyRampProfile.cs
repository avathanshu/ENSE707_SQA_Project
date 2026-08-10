namespace P2.LoadGenerator.Profiles;

/// <summary>
/// Linearly ramps concurrency from Start to Peak over the profile's duration.
/// Models gradually increasing traffic, e.g. the lead-up to a sale event.
/// </summary>
public sealed class SteadyRampProfile : ILoadProfile
{
    public string Name => "SteadyRamp";
    public TimeSpan Duration { get; }
    public int StartConcurrency { get; }
    public int PeakConcurrency { get; }

    public SteadyRampProfile(TimeSpan duration, int startConcurrency, int peakConcurrency)
    {
        if (peakConcurrency < startConcurrency)
            throw new ArgumentException("Peak concurrency must be >= start concurrency.", nameof(peakConcurrency));

        Duration = duration;
        StartConcurrency = startConcurrency;
        PeakConcurrency = peakConcurrency;
    }

    public int GetTargetConcurrency(TimeSpan elapsed)
    {
        if (elapsed >= Duration) return PeakConcurrency;

        var progress = elapsed.TotalMilliseconds / Duration.TotalMilliseconds;
        var value = StartConcurrency + (PeakConcurrency - StartConcurrency) * progress;
        return (int)Math.Round(value);
    }
}