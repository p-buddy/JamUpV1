using Unity.Collections;
using AudioEventSystem.DTO;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace AudioEventSystem.Jobs
{
    [BurstCompile]
    public struct AbridgeMonoEventsJob : IJobParallelFor
    {
        [NativeDisableContainerSafetyRestriction]
        public NativeArray<AudioEvent> SortedAudioEvents;

        [ReadOnly] 
        public int Length;
        
        public void Execute()
        {
            if (SortedAudioEvents.Length <= 1)
            {
                return;
            }
            
            for (var index = 0; index < SortedAudioEvents.Length - 1; index++)
            {
                AudioEvent current = SortedAudioEvents[index];
                double maxPlayTime = SortedAudioEvents[index + 1].Details.StartTime - current.Details.StartTime;
                SortedAudioEvents[index] = new AudioEvent(current, new PlayEventDetails(current.Details, maxPlayTime));
            }
        }

        public void Execute(int index)
        {
            if (Length <= 1 || index >= Length - 1)
            {
                return;
            }
            
            AudioEvent current = SortedAudioEvents[index];
            double maxPlayTime = SortedAudioEvents[index + 1].Details.StartTime - current.Details.StartTime;
            SortedAudioEvents[index] = new AudioEvent(current, new PlayEventDetails(current.Details, maxPlayTime));
        }
    }
}