using System;
using Unity.Entities;

public readonly struct TrackAliasComponent : IComponentData
{
    public Guid ID { get; }
    public bool Mono { get; }

    public TrackAliasComponent(Guid id, bool isMono)
    {
        ID = id;
        Mono = isMono;
    }
}