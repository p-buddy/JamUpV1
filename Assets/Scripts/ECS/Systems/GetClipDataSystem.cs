using System;
using System.Collections.Generic;
using ClipManagement;
using ECS.Components;
using ECS.Components.Tags;
using ECS.Systems.Jobs;
using ECS.Systems.Jobs.DTO;
using MonoBehaviours;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECS.Systems
{
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

        private Dictionary<float, NativeList<MonoClipData>> monoClipDataByPitch;
        private Dictionary<float, JobHandle> monoHandlesByPitch;
        
        private Dictionary<float, NativeList<StereoClipData>> stereoClipDataByPitch;
        private Dictionary<float, JobHandle> stereoHandlesByPitch;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            // Find the ECB system once and store it for later usage
            m_EndSimulationEcbSystem = World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            archetype = entityManager.CreateArchetype(typeof(ClipMicroSample));
            
            monoClipDataByPitch = new Dictionary<float, NativeList<MonoClipData>>();
            monoHandlesByPitch = new Dictionary<float, JobHandle>();
            
            stereoClipDataByPitch = new Dictionary<float, NativeList<StereoClipData>>();
            stereoHandlesByPitch = new Dictionary<float, JobHandle>();

            runningJobs = new List<RunningJob>();
        }

        private bool done = false;
        protected override void OnUpdate()
        {
            return;
            
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

            Entities.WithStructuralChanges().WithoutBurst().WithAll<UnMergedClipComponent>().WithNone<MonoAwaitingProcessing>()
                .ForEach((Entity entity, int entityInQueryIndex, in ClipAliasComponent clipAlias, in PlayEventComponent eventDetails) =>
            {
                if (!clipRegister.TryGetClip(clipAlias, out AudioClip clip))
                {
                    return;
                }
                entityManager.RemoveComponent<UnMergedClipComponent>(entity);
                entityManager.AddComponent<BeingProcessedComponent>(entity);
                float length = (clipAlias.LengthSet) ? clipAlias.Length : clip.length;
                if (!clipAlias.LengthSet)
                {
                    entityManager.SetComponentData(entity, new ClipAliasComponent(clipAlias, clip.length));
                }
                
                
                float volume = eventDetails.Volume;
                float pitch = eventDetails.Pitch;
                int frequency = clip.frequency;
                double startTime = eventDetails.TrackTime;
                double endTime = startTime + length;

                float[] data = new float[clip.samples * clip.channels];
                clip.GetData(data, 0);

                float lengthDelta = (clipAlias.LengthSet) ? length - clipAlias.Length : 0f;

                int sampleCount = (lengthDelta > 0)
                    ? (clip.samples - (int) (lengthDelta * frequency)) * clip.channels
                    : data.Length;
                NativeArray<float> samples = new NativeArray<float>(sampleCount, Allocator.TempJob);
                NativeArray<float>.Copy(data, 0, samples, 0, sampleCount);
                
                ChannelType channel = (clip.channels == 0) ? ChannelType.Mono : ChannelType.Stereo;
                
                var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();
                switch (channel)
                {
                    case ChannelType.Mono:
                        if (!monoClipDataByPitch.ContainsKey(pitch))
                        {
                            monoClipDataByPitch[pitch] = new NativeList<MonoClipData>(Allocator.Persistent);
                        }
                        if (!monoHandlesByPitch.ContainsKey(pitch))
                        {
                            monoHandlesByPitch[pitch] = new JobHandle();
                        }
                        #region Combine with existing clips
                        NativeList<int> overlappingMonoIndexes = new NativeList<int>(Allocator.TempJob);
                        AddToExistingMonoClipData addExistingMonoJob = new AddToExistingMonoClipData()
                        {
                            SampleData = samples,
                            ClipData = monoClipDataByPitch[pitch],
                            AddedIndexes = overlappingMonoIndexes,
                            StartTime = startTime,
                            EndTime = endTime,
                            SampleRate = frequency,
                            Volume = volume
                        };
                        JobHandle addToExistingMonoHandle = addExistingMonoJob.Schedule(monoClipDataByPitch[pitch].Length, JobBatchCount, monoHandlesByPitch[pitch]);
                        #endregion Combine with existing clips

                        #region Add Clips at Unique Time
                        AddUniqueClipMonoDataJob addUniqueMonoJob = new AddUniqueClipMonoDataJob()
                        {
                            SampleData = samples,
                            ClipData = monoClipDataByPitch[pitch],
                            AlreadyAddedIndexes = overlappingMonoIndexes,
                            Frequency = frequency,
                            StartTime = startTime,
                            Volume = volume
                        };
                        JobHandle addUniqueMonoHandle = addUniqueMonoJob.Schedule(addToExistingMonoHandle);
                        JobHandle disposeOverlappingMono = overlappingMonoIndexes.Dispose(addUniqueMonoHandle);
                        #endregion
                        
                        #region Sort
                        JobHandle sortMonoHandle = monoClipDataByPitch[pitch].Sort(addUniqueMonoHandle);
                        #endregion Sort

                        #region Consolidate
                        ConsolidateMonoClipsJob consolidateMonoJob = new ConsolidateMonoClipsJob()
                        {
                            ClipData = monoClipDataByPitch[pitch]
                        };
                        JobHandle consolidateMonoHandle = consolidateMonoJob.Schedule(sortMonoHandle);
                        #endregion
                        
                        JobHandle combinedMono = JobHandle.CombineDependencies(consolidateMonoHandle, disposeOverlappingMono);
                        runningJobs.Add(new RunningJob(combinedMono));
                        monoHandlesByPitch[pitch] = combinedMono;
                        break;
                    case ChannelType.Stereo:
                        if (!stereoClipDataByPitch.ContainsKey(pitch))
                        {
                            stereoClipDataByPitch[pitch] = new NativeList<StereoClipData>(Allocator.Persistent);
                        }
                        if (!stereoHandlesByPitch.ContainsKey(pitch))
                        {
                            stereoHandlesByPitch[pitch] = new JobHandle();
                        }
                        #region Combine with existing clips
                        NativeList<int> overlappingIndexes = new NativeList<int>(Allocator.TempJob);
                        AddToExistingStereoClipData addExistingJob = new AddToExistingStereoClipData()
                        {
                            SampleData = samples,
                            ClipData = stereoClipDataByPitch[pitch],
                            AddedIndexes = overlappingIndexes,
                            StartTime = startTime,
                            EndTime = endTime,
                            SampleRate = frequency,
                            Volume = volume
                        };
                        JobHandle addToExistingHandle = addExistingJob.Schedule(stereoClipDataByPitch[pitch].Length, JobBatchCount, stereoHandlesByPitch[pitch]);
                        #endregion Combine with existing clips

                        #region Add Clips at Unique Time
                        AddUniqueClipStereoDataJob addUniqueJob = new AddUniqueClipStereoDataJob()
                        {
                            SampleData = samples,
                            ClipData = stereoClipDataByPitch[pitch],
                            AlreadyAddedIndexes = overlappingIndexes,
                            Frequency = frequency,
                            StartTime = startTime,
                            Volume = volume
                        };
                        JobHandle addUniqueHandle = addUniqueJob.Schedule(addToExistingHandle);
                        JobHandle disposeOverlappingStereo = overlappingIndexes.Dispose(addUniqueHandle);
                        #endregion
                        
                        #region Sort
                        JobHandle sortHandle = stereoClipDataByPitch[pitch].Sort(addUniqueHandle);
                        #endregion Sort

                        #region Consolidate
                        ConsolidateStereoClipsJob consolidateJob = new ConsolidateStereoClipsJob()
                        {
                            ClipData = stereoClipDataByPitch[eventDetails.Pitch]
                        };
                        JobHandle consolidateHandle = consolidateJob.Schedule(sortHandle);
                        #endregion
                        
                        JobHandle combinedStereo = JobHandle.CombineDependencies(consolidateHandle, disposeOverlappingStereo);
                        runningJobs.Add(new RunningJob(combinedStereo));
                        stereoHandlesByPitch[pitch] = combinedStereo;
                        break;
                }
            }).Run();
            JobHandle.ScheduleBatchedJobs();
        }
    }
}