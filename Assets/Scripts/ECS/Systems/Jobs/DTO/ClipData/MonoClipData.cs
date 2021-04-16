using UnityEngine;

namespace ECS.Systems.Jobs.DTO
{
    public readonly struct MonoClipData : IClipData<MonoClipData>
    {
        public float MonoAmplitude { get; }
        public MonoClipData(float monoAmplitude)
        {
            MonoAmplitude = Mathf.Clamp(monoAmplitude, -1f, 1f);
        }
        public MonoClipData Sum(in MonoClipData other)
        {
            float mono = MonoAmplitude + other.MonoAmplitude;
            mono = Mathf.Clamp(mono, -1f, 1f);

            return new MonoClipData(mono);
        }

        public MonoClipData Difference(in MonoClipData other)
        {
            float mono = MonoAmplitude - other.MonoAmplitude;
            mono = Mathf.Clamp(mono, -1f, 1f);

            return new MonoClipData(mono);
        }
    }
}