public readonly struct PlayEventDetails
{
    public double TrackTime { get; }
    public float Volume { get; }

    public PlayEventDetails(float atTime, float volume)
    {
        TrackTime = atTime;
        Volume = volume;
    }
}
