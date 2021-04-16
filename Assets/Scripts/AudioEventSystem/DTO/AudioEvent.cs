using System;
using UnityEngine;

namespace AudioEventSystem.DTO
{
    public readonly struct AudioEvent : IComparable<AudioEvent>
    {
        public ClipAliasComponent ClipAlias { get; }
        public TrackAliasComponent TrackAlias { get; }
        public SampleState SampleState { get; }
        
        [field: SerializeField]
        public PlayEventDetails Details { get; }
        
        public AudioEvent(in ClipAliasComponent clip, in TrackAliasComponent track, in SampleState state,
            in PlayEventDetails modifiedDetails)
        {
            ClipAlias = clip;
            TrackAlias = track;
            SampleState = state;
            Details = modifiedDetails;
        }
        
        public AudioEvent(in AudioEvent baseAudioEvent, in PlayEventDetails modifiedDetails)
        {
            ClipAlias = baseAudioEvent.ClipAlias;
            TrackAlias = baseAudioEvent.TrackAlias;
            SampleState = baseAudioEvent.SampleState;
            Details = modifiedDetails;
        }

        public int CompareTo(AudioEvent other)
        {
            return Details.StartTime.CompareTo(other.Details.StartTime);
        }
    }
}