using ECS.Systems.Jobs.DTO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace ECS.Systems.Jobs
{
    [BurstCompile]
    public struct AddToClipDataJob<T, U> : IJobParallelFor where T : unmanaged, IClipData<T>
        where U : unmanaged, IClipDataFactory<T>
    {
        [ReadOnly] 
        public int StartOffset;

        public NativeList<T> ClipData;
        
        [ReadOnly]
        public NativeArray<float> Samples;
        
        [ReadOnly]
        public U Factory;

        [ReadOnly]
        public float Volume;

        public void Execute(int index)
        {
            int clipIndex = StartOffset + index;
            ClipData[clipIndex] = ClipData[clipIndex].Sum(Factory.Get(Samples, index, Volume));
        }
    }
}