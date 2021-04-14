using System;
using Utility;

namespace ECS.Systems.Jobs.DTO
{
    public readonly struct IntervalIntersection
    {
        public OptionalTimeInterval Overlap { get; }
        public OptionalTimeInterval Left { get; }
        public OptionalTimeInterval Right { get; }

        public IntervalIntersection(in TimeInterval baseInterval, in TimeInterval queryInterval, double timeResolution = (double)1/44100)
        {
            double start = Math.Max(baseInterval.StartTime, queryInterval.StartTime);
            double end = Math.Min(baseInterval.EndTime, queryInterval.EndTime);
            
            if (start < end)
            {
                Overlap = new OptionalTimeInterval(new TimeInterval(start, end), IntervalOrigin.IntersectingIntervals);
            }
            else
            {
                Overlap = default;
                BurstAssert.IsTrue(!Overlap.Exists);
            }

            if (Overlap.Exists)
            {
                if (start > baseInterval.StartTime)
                {
                    Left = new OptionalTimeInterval(new TimeInterval(baseInterval.StartTime, start), IntervalOrigin.BaseInterval);
                }
                else if (start > queryInterval.StartTime)
                {
                    Left = new OptionalTimeInterval(new TimeInterval(queryInterval.StartTime, start), IntervalOrigin.QueryInterval);
                }
                else
                {
                    Left = default;
                    BurstAssert.IsTrue(!Left.Exists);
                }
            }
            else
            {
                Left = (baseInterval.StartTime < queryInterval.StartTime)
                    ? new OptionalTimeInterval(baseInterval, IntervalOrigin.BaseInterval)
                    : new OptionalTimeInterval(queryInterval, IntervalOrigin.QueryInterval);
            }


            if (Overlap.Exists)
            {
                if (end < baseInterval.EndTime)
                {
                    Right = new OptionalTimeInterval(new TimeInterval(end, baseInterval.EndTime), IntervalOrigin.BaseInterval);
                }
                else if (end < queryInterval.EndTime)
                {
                    Right = new OptionalTimeInterval(new TimeInterval(end, queryInterval.EndTime), IntervalOrigin.QueryInterval);
                }
                else
                {
                    Right = default;
                    BurstAssert.IsTrue(!Right.Exists);
                } 
            }
            else
            {
                Right = (baseInterval.EndTime > queryInterval.EndTime)
                    ? new OptionalTimeInterval(baseInterval, IntervalOrigin.BaseInterval)
                    : new OptionalTimeInterval(queryInterval, IntervalOrigin.QueryInterval);
            }

            BurstAssert.IsNotTrue(!Overlap.Exists && Left.Origin == Right.Origin);
        }
    }
}