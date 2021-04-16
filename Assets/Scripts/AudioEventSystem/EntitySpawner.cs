using System;
using System.Collections;
using System.Collections.Generic;
using ECS.Components.Tags;
using Unity.Collections;
using Unity.Entities;
using AudioEventSystem.DTO;
using AudioEventSystem.Jobs;
using DataStructures.PriorityQueue;
using Unity.Jobs;

public class EntitySpawner : IAudioEventSpawner, IDisposable
{
    private readonly EntityArchetype audioEventArchetype;
    private EntityManager entityManager;
    private NativeList<Entity> audioEventEntities;
    private Dictionary<Guid, NativeList<AudioEvent>> MonoAudioEvents;

    public EntitySpawner()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        audioEventArchetype = entityManager.CreateArchetype(typeof(PlayEventComponent),
            typeof(ClipAliasComponent),
            typeof(TrackAliasComponent),
            typeof(UnMergedClipComponent));
        audioEventEntities = new NativeList<Entity>(Allocator.Persistent);

        MonoAudioEvents = new Dictionary<Guid, NativeList<AudioEvent>>();
    }

    public void EnqueueAudioEvent(in AudioEvent audioEvent)
    {
        if (audioEvent.TrackAlias.Mono)
        {
            if (MonoAudioEvents.TryGetValue(audioEvent.TrackAlias.ID, out NativeList<AudioEvent> events))
            {
                events.Add(audioEvent);
            }
            else
            {
                MonoAudioEvents[audioEvent.TrackAlias.ID] = new NativeList<AudioEvent>(Allocator.Persistent);
                MonoAudioEvents[audioEvent.TrackAlias.ID].Add(audioEvent);
            }
        }
        else
        {
            Spawn(audioEvent);
        }
    }

    public IEnumerator SpawnAll(Action onComplete)
    {
        yield return ProcessMonoEvents(onComplete);
    }

    private IEnumerator ProcessMonoEvents(Action onComplete)
    {
        JobHandle commonHandle = new JobHandle();
        PriorityQueue<(JobHandle handle, Guid key), int> handleQueue = new PriorityQueue<(JobHandle, Guid), int>(1);
        int maxSpawnsPerFrame = 10000;
        int loop = 1;
        foreach (KeyValuePair<Guid, NativeList<AudioEvent>> audioEventPair in MonoAudioEvents)
        {
            int eventsLength = audioEventPair.Value.Length;
            JobHandle sortHandle = audioEventPair.Value.Sort(new JobHandle());
            AbridgeMonoEventsJob abridgeForMonoJob = new AbridgeMonoEventsJob
            {
                SortedAudioEvents = audioEventPair.Value,
                Length = eventsLength
            };
            JobHandle handleForIteration = abridgeForMonoJob.Schedule(eventsLength, 64, sortHandle);
            handleQueue.Insert((handleForIteration, audioEventPair.Key), loop * eventsLength);
            commonHandle = JobHandle.CombineDependencies(commonHandle, handleForIteration);

            yield return null;

            (JobHandle handle, Guid key) queueItem = handleQueue.Top();
            if (queueItem.handle.IsCompleted)
            {
                handleQueue.Pop();
                for (var index = 0; index < MonoAudioEvents[queueItem.key].Length; index++)
                {
                    var audioEvent = MonoAudioEvents[queueItem.key][index];
                    Spawn(audioEvent);
                    if (index % maxSpawnsPerFrame == 0)
                    {
                        yield return null;
                    }
                }

                MonoAudioEvents[queueItem.key].Dispose();
                yield return null;
            }
            
            loop++;
        }

        while (handleQueue.HasNext())
        {
            (JobHandle handle, Guid key) queueItem = handleQueue.Pop();
            queueItem.handle.Complete();
            for (var index = 0; index < MonoAudioEvents[queueItem.key].Length; index++)
            {
                var audioEvent = MonoAudioEvents[queueItem.key][index];
                Spawn(audioEvent);
            }

            MonoAudioEvents[queueItem.key].Dispose();
            yield return null;
        }
        
        MonoAudioEvents.Clear();
        onComplete.Invoke();
    }

    private void Spawn(in AudioEvent audioEvent)
    {
        Entity entity = entityManager.CreateEntity(audioEventArchetype);
        entityManager.SetComponentData(entity, new PlayEventComponent(audioEvent.SampleState, audioEvent.Details));
        entityManager.SetComponentData(entity, audioEvent.ClipAlias);
        entityManager.SetComponentData(entity, audioEvent.TrackAlias);
    }

    public void ClearAudioEvents()
    {
        entityManager.DestroyEntity(audioEventEntities.AsArray());
    }

    public void Dispose()
    {
        audioEventEntities.Dispose();
    }
}
