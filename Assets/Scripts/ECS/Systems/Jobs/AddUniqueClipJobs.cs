using ECS.Components.Tags;
using ECS.Systems.Jobs.DTO;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ECS.Systems.Jobs
{
    #region Stereo
    [BurstCompile]
    public struct AddUniqueClipStereoDataJob : IJob
    {
        [ReadOnly] 
        [DeallocateOnJobCompletion]
        public NativeArray<float> SampleData;
        public NativeList<StereoClipData> ClipData;
        [ReadOnly]
        public NativeList<int> AlreadyAddedIndexes;

        [ReadOnly]
        public float Volume;
        [ReadOnly]
        public double StartTime;
        [ReadOnly]
        public int Frequency;
        
        public void Execute()
        {
            for (int index = 0; index < SampleData.Length; index+=2)
            {
                if (AlreadyAddedIndexes.Contains(index))
                {
                    continue;
                }
                float left = SampleData[index] * Volume;
                float right = SampleData[index + 1] * Volume;
                double time = StartTime + (double)index / Frequency;
                ClipData.Add(new StereoClipData(time, left, right));
            }
        }
    }
    #endregion Stereo

    #region Mono
    [BurstCompile]
    public struct AddUniqueClipMonoDataJob : IJob
    {
        [ReadOnly] 
        [DeallocateOnJobCompletion]
        public NativeArray<float> SampleData;
        public NativeList<MonoClipData> ClipData;
        [ReadOnly]
        public NativeList<int> AlreadyAddedIndexes;

        [ReadOnly]
        public float Volume;
        [ReadOnly]
        public double StartTime;
        [ReadOnly]
        public int Frequency;
        
        public void Execute()
        {
            for (int index = 0; index < SampleData.Length; index++)
            {
                if (AlreadyAddedIndexes.Contains(index))
                {
                    continue;
                }
                float mono = SampleData[index] * Volume;
                double time = StartTime + (double)index / Frequency;
                ClipData.Add(new MonoClipData(time, mono));
            }
        }
    }

    #endregion
}