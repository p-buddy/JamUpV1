using ECS.Systems.Jobs.DTO;
using Unity.Collections;
using Unity.Jobs;

namespace ECS.Systems.Jobs
{
    public struct FloatsFromClipData : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<StereoClipData> ClipData;
        
        [WriteOnly]
        public NativeArray<float> toFill;
        
        public void Execute(int index)
        {
            toFill[index] = (index % 2 == 0) ? ClipData[index / 2].LeftAmplitude : ClipData[index / 2].RightAmplitude;
        }
    }
}