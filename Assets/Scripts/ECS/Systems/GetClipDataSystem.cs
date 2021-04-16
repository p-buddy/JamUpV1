using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using ClipManagement;
using ECS.Components;
using ECS.Components.Tags;
using ECS.Systems.Jobs;
using ECS.Systems.Jobs.DTO;
using MonoBehaviours;
using MonoBehaviours.Utility;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECS.Systems
{
    public readonly struct ProcessClipHelper<T, U> where T : unmanaged, IClipData<T> where U : IClipDataFactory<T>
    {
        private static NativeQueue<NativeList<IndexRange>> IndexRangePool;
        private static NativeQueue<NativeArray<MinMaxIndex>> MinMaxPool;
        public TimeInterval ClipInterval { get; }
        public float Volume { get; }
        public int Frequency { get; }
        public float Pitch { get; }

        private readonly float[] sampleData;
        public ProcessClipHelper(AudioClip clip, in PlayEventComponent eventDetails)
        {
            Volume = eventDetails.Volume;
            Pitch = eventDetails.Pitch;
            Frequency = clip.frequency;
            double startTime = eventDetails.TrackTime;
            double endTime = startTime + Math.Min(eventDetails.MaxPlayTime, clip.length);
            ClipInterval = new TimeInterval(startTime, endTime);
            
            sampleData = new float[clip.samples * clip.channels];
            clip.GetData(sampleData, 0);
        }

        public IEnumerator Process(Dictionary<float, NativeList<TimeInterval>> intervalsByPitch,
            Dictionary<float, NativeList<T>> clipDataByPitch,
            Dictionary<float, JobHandle> intervalHandlesByPitch,
            Dictionary<float, JobHandle> clipHandlesByPitch)
        {
            NativeList<IndexRange>? rangesToAdd = null;
            NativeArray<MinMaxIndex>? minMax = null;
            JobHandle? forwardJobHandle = null;
            if (!intervalsByPitch.TryGetValue(Pitch, out NativeList<TimeInterval> intervals))
            {
                intervalsByPitch[Pitch] = new NativeList<TimeInterval>(Allocator.Persistent);
                intervalsByPitch[Pitch].Add(ClipInterval);
            }
            else
            {
                rangesToAdd = new NativeList<IndexRange>(Allocator.TempJob);
                yield return null;
                minMax = new NativeArray<MinMaxIndex>(1, Allocator.TempJob);
                yield return null;
                FindRangesToAddForwardJob forwardJob = new FindRangesToAddForwardJob(intervals,
                    rangesToAdd.Value,
                    ClipInterval,
                    Frequency,
                    false,
                    minMax.Value);
                forwardJobHandle = forwardJob.Schedule(intervalHandlesByPitch[Pitch]);
                intervalHandlesByPitch[Pitch] = forwardJobHandle.Value;
            }
            yield return null;

            if (forwardJobHandle.HasValue)
            {
                forwardJobHandle.Value.Complete();
            }
            
            yield return null;
            
            if (rangesToAdd.HasValue)
            {
                rangesToAdd.Value.Dispose();
            }

            if (minMax.HasValue)
            {
                minMax.Value.Dispose();
            }

            yield return null;

            ExpandNativeListJob<T> expandNativeListJob = new ExpandNativeListJob<T>();
            NativeArray<float> samples = new NativeArray<float>(sampleData.Length, Allocator.TempJob);
            NativeArray<float>.Copy(sampleData, 0, samples, 0, sampleData.Length);
        }
    }
    
    public class GetClipDataSystem : SystemBase
    {
        private struct RunningJob
        {
            public JobHandle Handle;
            public int FrameCountRemaining;

            public RunningJob(JobHandle handle)
            {
                Handle = handle;
                FrameCountRemaining = 4;
            }
        }
        private const int JobBatchCount = 16;
        private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

        private EntityManager entityManager;
        private EntityArchetype archetype;

        private List<RunningJob> runningJobs;

        private Dictionary<float, NativeList<TimeInterval>> monoIntervalsByPitch;
        private Dictionary<float, JobHandle> monoHandlesByPitch;
        
        private Dictionary<float, NativeList<TimeInterval>> stereoIntervalsByPitch;
        private Dictionary<float, JobHandle> stereoHandlesByPitch;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            // Find the ECB system once and store it for later usage
            m_EndSimulationEcbSystem = World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //archetype = entityManager.CreateArchetype(typeof());
            
            monoIntervalsByPitch = new Dictionary<float, NativeList<TimeInterval>>();
            monoHandlesByPitch = new Dictionary<float, JobHandle>();
            
            stereoIntervalsByPitch = new Dictionary<float, NativeList<TimeInterval>>();
            stereoHandlesByPitch = new Dictionary<float, JobHandle>();

            runningJobs = new List<RunningJob>();
        }

        private bool done = false;
        protected override void OnUpdate()
        {
            //return;
            
            if (!GameManager.Instance.TryFetch(out IClipRegister clipRegister))
            {
                return;
            }

            for (int index = runningJobs.Count - 1; index >= 0; index--)
            {
                RunningJob runningJob = runningJobs[index];
                runningJob.FrameCountRemaining = runningJob.FrameCountRemaining - 1;
                if (runningJob.FrameCountRemaining == 0)
                {
                    runningJob.Handle.Complete();
                    runningJobs.RemoveAt(index);
                }
            }

            Entities.WithStructuralChanges().WithoutBurst().WithAll<UnMergedClipComponent>()
                .ForEach((Entity entity, int entityInQueryIndex, in ClipAliasComponent clipAlias, in PlayEventComponent eventDetails) =>
            {
                if (!clipRegister.TryGetClip(clipAlias, out AudioClip clip))
                {
                    return;
                }
                entityManager.RemoveComponent<UnMergedClipComponent>(entity);
                entityManager.AddComponent<BeingProcessedComponent>(entity);

                ProcessClipHelper clipProcessor = new ProcessClipHelper(clip, in eventDetails);
                CoroutineProcessor.Instance.EnqueCoroutine(clipProcessor.Process())
                
            }).Run();
            JobHandle.ScheduleBatchedJobs();
        }
    }
}