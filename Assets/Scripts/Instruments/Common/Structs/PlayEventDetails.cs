using UnityEngine;

public readonly struct PlayEventDetails
{
    public double StartTime { get; }
    
    [field: SerializeField]
    public double MaxPlayTime { get; }
    public float Volume { get; }

    public PlayEventDetails(float atTime, float volume, float maxPlayTime = float.MaxValue)
    {
        StartTime = atTime;
        Volume = volume;
        MaxPlayTime = maxPlayTime;
    }
    
    public PlayEventDetails(in PlayEventDetails baseEventDetails, double modifiedMaxTime)
    {
        StartTime = baseEventDetails.StartTime;
        Volume = baseEventDetails.Volume;
        MaxPlayTime = modifiedMaxTime;
    }
}
