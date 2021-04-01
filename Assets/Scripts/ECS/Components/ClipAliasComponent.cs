using Unity.Entities;

public readonly struct ClipAliasComponent : IComponentData
{
    public int Index { get; }
    public ClipAliasComponent(int index)
    {
        Index = index;
    }
}