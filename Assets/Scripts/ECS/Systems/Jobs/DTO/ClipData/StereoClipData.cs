using UnityEngine;

namespace ECS.Systems.Jobs.DTO
{
    public readonly struct StereoClipData : IClipData<StereoClipData>
    {
        public float LeftAmplitude { get; }
        public float RightAmplitude { get; }
        
        public StereoClipData(float leftAmplitude, float rightAmplitude)
        {
            LeftAmplitude = Mathf.Clamp(leftAmplitude, -1f, 1f);
            RightAmplitude = Mathf.Clamp(rightAmplitude, -1f, 1f);
        }
        
        public StereoClipData Sum(in StereoClipData other)
        {
            float left = LeftAmplitude + other.LeftAmplitude;
            float right = RightAmplitude + other.RightAmplitude;
            left = Mathf.Clamp(left, -1f, 1f);
            right = Mathf.Clamp(right, -1f, 1f);
            
            return new StereoClipData(left, right);
        }

        public StereoClipData Difference(in StereoClipData other)
        {
            float left = LeftAmplitude - other.LeftAmplitude;
            float right = RightAmplitude - other.RightAmplitude;
            left = Mathf.Clamp(left, -1f, 1f);
            right = Mathf.Clamp(right, -1f, 1f);
            
            return new StereoClipData(left, right);
        }

        public override string ToString()
        {
            return $"[L: {LeftAmplitude}, R: {RightAmplitude}]";
        }
    }
}