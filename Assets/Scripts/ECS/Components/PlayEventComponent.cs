using Unity.Entities;
using UnityEngine;

public readonly struct PlayEventComponent : IComponentData
{
    [field: SerializeField]
    public double TrackTime { get; }
    
    [field: SerializeField]
    public double MaxPlayTime { get; }
    
    [field: SerializeField]
    public float Volume { get; }
    
    [field: SerializeField]
    public float Pitch { get; }

    public PlayEventComponent(SampleState state, PlayEventDetails details)
    {
        Pitch = state.SemitoneOffset.ToFrequency();
        Volume = details.Volume;
        TrackTime = details.StartTime;
        MaxPlayTime = details.MaxPlayTime;
    }
}