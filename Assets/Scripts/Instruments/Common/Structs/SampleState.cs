using System;

public readonly struct SampleState : IEquatable<SampleState>
{
    public int SemitoneOffset { get; }
    public double StartOffset { get; }
    public double EndOffset { get; }

    public SampleState(int semitoneOffset, double startOffset, double endOffset)
    {
        SemitoneOffset = semitoneOffset;
        StartOffset = startOffset;
        EndOffset = endOffset;
    }
    
    public override bool Equals(object obj)
    {
        return obj is SampleState other && Equals(other);
    }

    public bool Equals(SampleState other)
    {
        return SemitoneOffset == other.SemitoneOffset && StartOffset.Equals(other.StartOffset) && EndOffset.Equals(other.EndOffset);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = SemitoneOffset;
            hashCode = (hashCode * 397) ^ StartOffset.GetHashCode();
            hashCode = (hashCode * 397) ^ EndOffset.GetHashCode();
            return hashCode;
        }
    }
}
