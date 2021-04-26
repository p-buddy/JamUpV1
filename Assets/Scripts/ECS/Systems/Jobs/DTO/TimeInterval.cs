using System;

namespace ECS.Systems.Jobs.DTO
{
    public readonly struct TimeInterval: IComparable<TimeInterval>
    {
        public double StartTime { get; }
        public double EndTime { get; }
        public int ClipCount { get; }
        public double Length => EndTime - StartTime;

        public TimeInterval(double startTime, double endTime, int clipCount = 1)
        {
            StartTime = startTime;
            EndTime = endTime;
            ClipCount = clipCount;
        }
        
        public TimeInterval(in TimeInterval interval, int clipCount)
        {
            StartTime = interval.StartTime;
            EndTime = interval.EndTime;
            ClipCount = clipCount;
        }

        public int CompareTo(TimeInterval other)
        {
            return StartTime.CompareTo(other.StartTime);
        }

        public override string ToString()
        {
            return $"({StartTime} - {EndTime} [clip count: {ClipCount}])";
        }
    }
}