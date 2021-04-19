using System.Collections.Generic;
using ClipManagement;
using ECS.Components.Tags;
using ECS.Systems.Jobs.DTO;
using ECS.Systems.Utility;
using MonoBehaviours;
using MonoBehaviours.Utility;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECS.Systems
{
    public class GetClipDataSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

        private EntityManager entityManager;
        private EntityArchetype archetype;

        #region Mono fields
        private Dictionary<float, NativeList<TimeInterval>> monoIntervalsByPitch;
        private Dictionary<float, NativeList<MonoClipData>> monoClipDataByPitch;
        private Dictionary<float, JobHandle> monoIntervalHandlesByPitch;
        private Dictionary<float, JobHandle> monoClipHandlesByPitch;
        private Dictionary<float, int> monoProcessCountByPitch;
        #endregion

        #region Stereo fields
        private Dictionary<float, NativeList<TimeInterval>> stereoIntervalsByPitch;
        private Dictionary<float, NativeList<StereoClipData>> stereoClipDataByPitch;
        private Dictionary<float, JobHandle> stereoIntervalHandlesByPitch;
        private Dictionary<float, JobHandle> stereoClipHandlesByPitch;
        private Dictionary<float, int> stereoProcessCountByPitch;
        #endregion
        
        
        protected override void OnCreate()
        {
            base.OnCreate();
            // Find the ECB system once and store it for later usage
            m_EndSimulationEcbSystem = World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //archetype = entityManager.CreateArchetype(typeof());
            
            monoIntervalsByPitch = new Dictionary<float, NativeList<TimeInterval>>();
            monoClipDataByPitch = new Dictionary<float, NativeList<MonoClipData>>();
            monoIntervalHandlesByPitch = new Dictionary<float, JobHandle>();
            monoClipHandlesByPitch = new Dictionary<float, JobHandle>();
            monoProcessCountByPitch = new Dictionary<float, int>();
            
            stereoIntervalsByPitch = new Dictionary<float, NativeList<TimeInterval>>();
            stereoClipDataByPitch = new Dictionary<float, NativeList<StereoClipData>>();
            stereoIntervalHandlesByPitch = new Dictionary<float, JobHandle>();
            stereoClipHandlesByPitch = new Dictionary<float, JobHandle>();
            stereoProcessCountByPitch = new Dictionary<float, int>();
        }

        protected override void OnUpdate()
        {
            if (!GameManager.Instance.TryFetch(out IClipRegister clipRegister))
            {
                return;
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
                float pitch = eventDetails.Pitch;

                if (clip.channels == 2)
                {
                    if (!monoProcessCountByPitch.ContainsKey(pitch))
                    {
                        monoProcessCountByPitch[pitch] = 1;
                    }
                    
                    var clipProcessor = new ProcessClipHelper<StereoClipData, FloatToStereoData>(clip, in eventDetails);
                    CoroutineProcessor.Instance.EnqueCoroutine(clipProcessor.Process(stereoIntervalsByPitch,
                        stereoClipDataByPitch,
                        stereoIntervalHandlesByPitch,
                        stereoClipHandlesByPitch,
                        () => monoProcessCountByPitch[pitch] = monoProcessCountByPitch[pitch] - 1));
                }
                else
                {
                    if (!stereoProcessCountByPitch.ContainsKey(pitch))
                    {
                        stereoProcessCountByPitch[pitch] = 1;
                    }
                    var clipProcessor = new ProcessClipHelper<MonoClipData, FloatToMonoData>(clip, in eventDetails);
                    CoroutineProcessor.Instance.EnqueCoroutine(clipProcessor.Process(monoIntervalsByPitch,
                        monoClipDataByPitch,
                        monoIntervalHandlesByPitch,
                        monoClipHandlesByPitch,
                        () => stereoProcessCountByPitch[pitch] = stereoProcessCountByPitch[pitch] - 1));
                }
                
            }).Run();
            JobHandle.ScheduleBatchedJobs();
        }
    }
}