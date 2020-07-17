using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

public class AudioEventEntitySpawner : Singleton<AudioEventEntitySpawner>
{
    private EntityArchetype archetype;
    private EntityManager entityManager;
    private Material material;
    private Mesh mesh;

    public void SpawnAudioEvent(ClipProxy clip, float time)
    {
        Entity entity = entityManager.CreateEntity(archetype);
        entityManager.SetComponentData(entity, new PlayEvent
        {
            timeAfterStart = time
        });

        entityManager.SetComponentData(entity, new ClipReference
        {
            index = clip.Index
        });

        //entityManager.SetComponentData(entity, new RenderMesh)
    }

    private void SetArchetype()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        archetype = entityManager.CreateArchetype(
            typeof(PlayEvent),
            typeof(ClipReference)
            );
    }

    #region
    private void Awake()
    {
        SetArchetype();
    }
    #endregion
}
