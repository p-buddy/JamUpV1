using Unity.Collections;

namespace ECS.Systems.Jobs.DTO
{
    public interface IClipDataFactory<T> where T : unmanaged, IClipData<T>
    {
        T Get(NativeArray<float> samples, int index, float scale);
    }
}