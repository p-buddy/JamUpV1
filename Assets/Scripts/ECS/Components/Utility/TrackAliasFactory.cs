using System;

public static class TrackAliasFactory
{
    public static TrackAliasComponent Create(bool isMono)
    {
        return new TrackAliasComponent(Guid.NewGuid(), isMono);
    }
}
