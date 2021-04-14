namespace ECS.Systems.Jobs.DTO
{
    public readonly struct MinMaxIndex
    {
        public int Min { get; }
        public int Max { get; }

        public MinMaxIndex(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public override string ToString()
        {
            return $"[{Min}, {Max}]";
        }
    }
}