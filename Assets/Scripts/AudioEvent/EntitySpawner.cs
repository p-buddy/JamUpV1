using System;
using ECS.Components.Tags;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class EntitySpawner : IAudioEventSpawner, IDisposable
{
    private readonly EntityArchetype audioEventArchetype;
    private EntityManager entityManager;
    private NativeList<Entity> audioEvents;

    public EntitySpawner()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        audioEventArchetype = entityManager.CreateArchetype(typeof(PlayEventComponent),
            typeof(ClipAliasComponent),
            typeof(TrackAliasComponent),
            typeof(UnMergedClipComponent));
        audioEvents = new NativeList<Entity>(Allocator.Persistent);
    }

    public void SpawnAudioEvent(in ClipAliasComponent clip,
        in TrackAliasComponent track,
        in SampleState state,
        in PlayEventDetails details)
    {
        Entity entity = entityManager.CreateEntity(audioEventArchetype);
        entityManager.SetComponentData(entity, new PlayEventComponent(state, details));
        entityManager.SetComponentData(entity, clip);
        entityManager.SetComponentData(entity, track);
    }

    public void ClearAudioEvents()
    {
        entityManager.DestroyEntity(audioEvents.AsArray());
    }

    public void Dispose()
    {
        audioEvents.Dispose();
    }
}
