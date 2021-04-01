using System;
using Unity.Entities;

namespace ECS.Components
{
    public enum ChannelType
    {
        Mono,
        Stereo,
    }
    public readonly struct ClipMicroSample : IComponentData, IComparable<ClipMicroSample>
    {
        public float Left { get; }
        public float Right { get; }
        public float Mono { get; }
        public float Pitch { get; }
        public ChannelType ChannelType { get; }
        public int SamplePosition { get; }

        public ClipMicroSample(float mono, int samplePosition, float pitch)
        {
            Mono = mono;
            ChannelType = ChannelType.Mono;
            Pitch = pitch;
            SamplePosition = samplePosition;

            Left = default;
            Right = default;
        }
        
        public ClipMicroSample(float left, float right, int samplePosition, float pitch)
        {
            Left = left;
            Right = right;
            ChannelType = ChannelType.Stereo;
            Pitch = pitch;
            SamplePosition = samplePosition;

            Mono = default;
        }


        public int CompareTo(ClipMicroSample other)
        {
            return SamplePosition.CompareTo(other.SamplePosition);
        }
    }
}