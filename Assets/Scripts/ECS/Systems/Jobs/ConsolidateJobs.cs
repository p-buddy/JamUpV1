using System.Collections.Generic;
using ECS.Systems.Jobs.DTO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace ECS.Systems.Jobs
{
    [BurstCompile]
    public struct ConsolidateStereoClipsJob : IJob
    {
        public NativeList<StereoClipData> ClipData;
        public void Execute()
        {
            for (int index = 0; index < ClipData.Length - 1; index++)
            {
                if (ClipData[index].Time != ClipData[index + 1].Time)
                {
                    continue;
                }
                
                ClipData[index] = new StereoClipData(ClipData[index], ClipData[index + 1]);
                int sameValueIndex = index + 1;
                while (sameValueIndex != (ClipData.Length - 1) && ClipData[index].Time == ClipData[sameValueIndex + 1].Time)
                {
                    sameValueIndex++;
                    ClipData[index] = new StereoClipData(ClipData[index], ClipData[sameValueIndex]);
                }

                ClipData.RemoveRangeWithBeginEnd(index + 1, sameValueIndex);
                index = sameValueIndex;
            }
        }
    }
    
    [BurstCompile]
    public struct ConsolidateMonoClipsJob : IJob
    {
        public NativeList<MonoClipData> ClipData;
        public void Execute()
        {
            for (int index = 0; index < ClipData.Length - 1; index++)
            {
                if (ClipData[index].Time != ClipData[index + 1].Time)
                {
                    continue;
                }

                ClipData[index] = new MonoClipData(ClipData[index], ClipData[index + 1]);
                int sameValueIndex = index + 1;
                while (sameValueIndex != (ClipData.Length - 1) && ClipData[index].Time == ClipData[sameValueIndex + 1].Time)
                {
                    sameValueIndex++;
                    ClipData[index] = new MonoClipData(ClipData[index], ClipData[sameValueIndex]);
                }

                ClipData.RemoveRangeWithBeginEnd(index + 1, sameValueIndex);
                index = sameValueIndex;
            }
        }
    }
}