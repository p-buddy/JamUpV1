using ECS.Systems.Jobs.DTO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace ECS.Systems.Jobs
{
    #region Stereo
    [BurstCompile]
    public struct AddToExistingStereoClipData : IJobParallelFor
    {
        [ReadOnly] 
        public NativeArray<float> SampleData;
        [ReadOnly]
        public NativeList<StereoClipData> ClipData;
        [ReadOnly]
        public NativeList<int> AddedIndexes;

        [ReadOnly]
        public double StartTime;
        [ReadOnly]
        public double EndTime;
        [ReadOnly]
        public int SampleRate;
        [ReadOnly]
        public float Volume;
        
        public void Execute(int index)
        {
            if (!IsOverlapping(ClipData[index]))
            {
                return;
            }

            StereoClipData addedClip = ClipData[index];
            int sampleIndex = (int)((addedClip.Time - StartTime) * SampleRate) * 2;
            float left = SampleData[sampleIndex] * Volume;
            float right = SampleData[sampleIndex + 1] * Volume;
            ClipData[index] = new StereoClipData(addedClip.Time,
                addedClip.LeftAmplitude + left,
                addedClip.RightAmplitude + right);
            AddedIndexes.Add(sampleIndex);
        }

        private bool IsOverlapping(in StereoClipData alreadyAddedClipData)
        {
            return (alreadyAddedClipData.Time >= StartTime && alreadyAddedClipData.Time <= EndTime);
        }
    }
    #endregion Stereo

    #region Mono
    [BurstCompile]
    public struct AddToExistingMonoClipData : IJobParallelFor
    {
        [ReadOnly] 
        public NativeArray<float> SampleData;
        [ReadOnly]
        public NativeList<MonoClipData> ClipData;
        [ReadOnly]
        public NativeList<int> AddedIndexes;

        [ReadOnly]
        public double StartTime;
        [ReadOnly]
        public double EndTime;
        [ReadOnly]
        public int SampleRate;
        [ReadOnly]
        public float Volume;
        
        public void Execute(int index)
        {
            if (!IsOverlapping(ClipData[index]))
            {
                return;
            }

            MonoClipData addedClip = ClipData[index];
            int sampleIndex = (int)((addedClip.Time - StartTime) * SampleRate);
            float mono = SampleData[sampleIndex] * Volume;
            ClipData[index] = new MonoClipData(addedClip.Time, addedClip.MonoAmplitude + mono);
            AddedIndexes.Add(sampleIndex);
        }

        private bool IsOverlapping(in MonoClipData alreadyAddedClipData)
        {
            return (alreadyAddedClipData.Time >= StartTime && alreadyAddedClipData.Time <= EndTime);
        }
    }
    #endregion Mono
}