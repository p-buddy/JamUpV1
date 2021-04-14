using System;

namespace ECS.Systems.Jobs.DTO
{
    public readonly struct IndexRange
    {
        public int StartIndex { get; }
        public int EndIndex { get; }
        public int Count => EndIndex - StartIndex + 1;

        public IndexRange(double length, int startIndex, double resolution = 1 / (double)44100)
        {
            StartIndex = startIndex;
            EndIndex = startIndex + (int)Math.Round(1 / resolution * length) - 1;
        }
        
        public override string ToString()
        {
            return $"({StartIndex} - {EndIndex})";
        }
    }
}