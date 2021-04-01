using System;
using Unity.Entities;

public readonly struct TrackAliasComponent : IComponentData
{
    public Guid ID { get; }

    public TrackAliasComponent(Guid id)
    {
        ID = id;
    }
}