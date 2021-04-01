using System;

public static class TrackAliasFactory
{
    public static TrackAliasComponent Create()
    {
        return new TrackAliasComponent(Guid.NewGuid());
    }
}
