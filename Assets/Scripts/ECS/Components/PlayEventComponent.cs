using Unity.Entities;

public readonly struct PlayEventComponent : IComponentData
{
    public double TrackTime { get; }
    public float Volume { get; }
    public float Pitch { get; }

    public PlayEventComponent(SampleState state, PlayEventDetails details)
    {
        Pitch = state.SemitoneOffset.ToFrequency();
        Volume = details.Volume;
        TrackTime = details.TrackTime;
    }
    
    public PlayEventComponent(float pitch, float volume, float trackTime)
    {
        Pitch = pitch;
        Volume = volume;
        TrackTime = trackTime;
    }
}