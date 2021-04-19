using ECS.Systems.Jobs.DTO;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities.UniversalDelegates;
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
        public float Volume;

        public AddToClipDataJob(int startOffset, NativeList<T> clipData, NativeArray<float> samples, float volume)
        {
            StartOffset = startOffset;
            ClipData = clipData;
            Samples = samples;
            Volume = volume;
        }

        public void Execute(int index)
        {
            int clipIndex = StartOffset + index;
            ClipData[clipIndex] = ClipData[clipIndex].Sum(new U().Get(Samples, index, Volume));
        }
    }
}