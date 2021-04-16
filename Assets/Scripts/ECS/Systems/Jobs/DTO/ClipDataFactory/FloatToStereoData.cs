using System;
using ECS.Components;
using Unity.Collections;
using Utility;

namespace ECS.Systems.Jobs.DTO
{
    public readonly struct FloatToStereoData : IClipDataFactory<StereoClipData>
    {
        public StereoClipData Get(NativeArray<float> samples, int index, float scale)
        {
            int sampleIndex = index * 2;
            float left = samples[sampleIndex] * scale;
            float right = samples[sampleIndex + 1] * scale;
            return new StereoClipData(left, right);
        }
    }
}