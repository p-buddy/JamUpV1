using System;
using ECS.Systems.Jobs.DTO;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Utility;

namespace ECS.Systems.Jobs
{
    [BurstCompile]
    public struct FindRangesToAddForwardJob : IJob
    {
        public NativeList<TimeInterval> Intervals;
        [WriteOnly]
        public NativeList<IndexRange> RangesToAdd;

        [ReadOnly]
        public TimeInterval IntervalQuery;
        
        [ReadOnly]
        public int Frequency;
        
        [ReadOnly]
        public bool RangesWillBeIteratedBackwards;
    
        [WriteOnly]
        public NativeArray<MinMaxIndex> MinMax;

        public void Execute()
        {
            if (Intervals.Length == 0)
            {
                Intervals.Add(new TimeInterval(IntervalQuery, 1));
                return;
            }
            
            int previousSampleIndex = -1;
            TimeInterval remainingQuery = IntervalQuery;
            int totalIntervalsAdded = 0;
            int samplesAdded = 0;
            int startingLength = Intervals.Length;
            int minSampleIndex = Int32.MaxValue;
            for (int index = 0; index < startingLength; index++)
            {
                int lengthAtStart = Intervals.Length;
                int currentIntervalIndex = index + totalIntervalsAdded;
                int currentSampleIndex = previousSampleIndex + 1;
                IndexRange currentRange = new IndexRange(Intervals[currentIntervalIndex].Length, currentSampleIndex, Frequency.Inverse());
                int currentClipCount = Intervals[currentIntervalIndex].ClipCount;
                
                IntervalIntersection intersection = new IntervalIntersection(Intervals[currentIntervalIndex], remainingQuery, Frequency.Inverse());

                EvaluateLeftInterval(intersection, currentIntervalIndex, currentSampleIndex, ref samplesAdded);
                EvaluateOverlappingInterval(intersection, currentIntervalIndex, currentClipCount);
                EvaluateRightInterval(intersection, ref remainingQuery, currentIntervalIndex);

                minSampleIndex = (intersection.Left.Exists && intersection.Left.Origin == IntervalOrigin.QueryInterval)
                    ? Math.Min(minSampleIndex, currentSampleIndex)
                    : minSampleIndex;
                minSampleIndex = (intersection.Overlap.Exists)
                    ? Math.Min(minSampleIndex, currentSampleIndex + intersection.Left.Interval.ToIndexCount(Frequency))
                    : minSampleIndex;

                totalIntervalsAdded += (Intervals.Length - lengthAtStart);
                previousSampleIndex = currentRange.EndIndex + (RangesWillBeIteratedBackwards ? 0 : samplesAdded);
                
                if (EarlyExit(intersection))
                {
                    remainingQuery = default;
                    break;
                }
            }

            if (remainingQuery.Length > 0)
            {
                Intervals.Add(remainingQuery);
                RangesToAdd.Add(new IndexRange(remainingQuery.Length, previousSampleIndex + 1, Frequency.Inverse()));
                minSampleIndex = Math.Min(minSampleIndex, previousSampleIndex + 1);
            }

            MinMax[0] = new MinMaxIndex(minSampleIndex, IntervalQuery.ToEndIndex(minSampleIndex, Frequency));
        }

        private void EvaluateLeftInterval(IntervalIntersection intersection, int currentIntervalIndex, int currentSampleIndex, ref int samplesAdded)
        {
            if (intersection.Left.Exists)
            {
                if (intersection.Left.Origin == IntervalOrigin.QueryInterval)
                {
                    IndexRange range = new IndexRange(intersection.Left.Interval.Length, currentSampleIndex, Frequency.Inverse());
                    RangesToAdd.Add(range);
                    samplesAdded += range.Count;
                }
                
                Intervals[currentIntervalIndex] = intersection.Left.Interval;
            }
        }

        private void EvaluateOverlappingInterval(IntervalIntersection intersection, int currentIntervalIndex, int clipCount)
        {
            if (intersection.Overlap.Exists)
            {
                if (intersection.Left.Exists)
                {
                    Intervals.ExpandAtIndex(currentIntervalIndex + 1);
                    Intervals[currentIntervalIndex + 1] = new TimeInterval(intersection.Overlap.Interval, clipCount + 1);
                }
                else
                {
                    Intervals[currentIntervalIndex] = new TimeInterval(intersection.Overlap.Interval, clipCount + 1);
                }
            }
        }
        
        private void EvaluateRightInterval(IntervalIntersection intersection, ref TimeInterval remainingQuery, int currentIntervalIndex)
        {
            if (intersection.Right.Exists)
            {
                if (intersection.Right.Origin == IntervalOrigin.QueryInterval)
                {
                    remainingQuery = intersection.Right.Interval;
                }
                
                if (intersection.Right.Origin == IntervalOrigin.BaseInterval)
                {
                    int indexOffset = intersection.Left.Exists && intersection.Overlap.Exists ? 2 : 1;  
                    Intervals.ExpandAtIndex(currentIntervalIndex + indexOffset);
                    Intervals[currentIntervalIndex + indexOffset] = new TimeInterval(intersection.Right.Interval, intersection.Right.Interval.ClipCount);;
                }
            }
        }

        private bool EarlyExit(IntervalIntersection intersection)
        {
            return !intersection.Right.Exists || intersection.Right.Origin == IntervalOrigin.BaseInterval;
        }
    }
}