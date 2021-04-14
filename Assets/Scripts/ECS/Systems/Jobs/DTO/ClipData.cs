using System;
using ECS.Components;
using UnityEngine;

namespace ECS.Systems.Jobs.DTO
{
    public interface IClipData
    {
        double Time { get; }
    }
    
    public readonly struct StereoClipData : IComparable<StereoClipData>, IClipData
    {
        public double Time { get; }
        public float LeftAmplitude { get; }
        public float RightAmplitude { get; }

        public StereoClipData(double time, float leftAmplitude, float rightAmplitude)
        {
            Time = time;
            LeftAmplitude = Mathf.Clamp(leftAmplitude, -1f, 1f);
            RightAmplitude = Mathf.Clamp(rightAmplitude, -1f, 1f);
        }

        public StereoClipData(in StereoClipData lfs, in StereoClipData rhs)
        {
            float left = lfs.LeftAmplitude + rhs.LeftAmplitude;
            float right = lfs.RightAmplitude + rhs.RightAmplitude;
            
            Time = lfs.Time;
            LeftAmplitude = Mathf.Clamp(left, -1f, 1f);
            RightAmplitude = Mathf.Clamp(right, -1f, 1f);
        }

        public int CompareTo(StereoClipData other)
        {
            return Time.CompareTo(other.Time);
        }
    }

    public readonly struct MonoClipData : IComparable<MonoClipData>
    {
        public double Time { get; }
        public float MonoAmplitude { get; }

        public MonoClipData(double time, float monoAmplitude)
        {
            Time = time;
            MonoAmplitude = Mathf.Clamp(monoAmplitude, -1f, 1f);
        }
        
        public MonoClipData(in MonoClipData lfs, in MonoClipData rhs)
        {
            float mono = lfs.MonoAmplitude + rhs.MonoAmplitude;
            Time = lfs.Time;
            MonoAmplitude = Mathf.Clamp(mono, -1f, 1f);
        }
        
        public int CompareTo(MonoClipData other)
        {
            return Time.CompareTo(other.Time);
        }
    }
}