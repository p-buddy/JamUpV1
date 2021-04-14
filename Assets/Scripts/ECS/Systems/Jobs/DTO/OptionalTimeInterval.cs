namespace ECS.Systems.Jobs.DTO
{
    public readonly struct OptionalTimeInterval
    {
        public TimeInterval Interval { get; }
        public bool Exists { get; }
        public IntervalOrigin Origin { get; }

        public OptionalTimeInterval(in TimeInterval interval, in IntervalOrigin origin)
        {
            Interval = interval;
            Exists = true;
            Origin = origin;
        }
    }
}