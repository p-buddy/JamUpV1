using ECS.Components;
using Unity.Entities;

public readonly struct ClipAliasComponent : IComponentData
{
    public int Index { get; }
    public float Length { get; }
    public bool LengthSet { get; }
    public ClipAliasComponent(int index)
    {
        Index = index;
        Length = -1f;
        LengthSet = false;
    }
    
    public ClipAliasComponent(in ClipAliasComponent clipAlias, float legnth)
    {
        Index = clipAlias.Index;
        Length = legnth;
        LengthSet = true;
    }
}