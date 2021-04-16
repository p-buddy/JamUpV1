using Unity.Collections;

namespace ECS.Systems.Jobs.DTO
{
    public readonly struct FloatToMonoData : IClipDataFactory<MonoClipData>
    {
        public MonoClipData Get(NativeArray<float> samples, int index, float scale)
        {
            return new MonoClipData(samples[index] * scale);
        }
    }
}