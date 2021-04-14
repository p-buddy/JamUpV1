namespace ECS.Systems.Jobs.DTO
{
    public static class IntervalHelper
    {
        public static string ToPrettyString(this in OptionalTimeInterval optionalInterval)
        {
            string output = (!optionalInterval.Exists)
                ? "Non-existent"
                : $"Origin: {optionalInterval.Origin}; Start: {optionalInterval.Interval.StartTime}; End: {optionalInterval.Interval.EndTime}";
            return output;
        }
        
        public static string ToPrettyString(this in IntervalIntersection intersection)
        {
            return $"Overlap: [{intersection.Overlap.ToPrettyString()}]\nLeft: [{intersection.Left.ToPrettyString()}]\nRight: [{intersection.Right.ToPrettyString()}]";
        }
    }
}