using AudioPlayBack;
using ClipManagement;
using ECS.Components;
using ECS.Components.Tags;
using MonoBehaviours;
using Unity.Entities;
using UnityEngine;

namespace ECS.Systems
{
    public class GetClipDataSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

        private EntityManager entityManager;
        private EntityArchetype archetype;
        protected override void OnCreate()
        {
            base.OnCreate();
            // Find the ECB system once and store it for later usage
            m_EndSimulationEcbSystem = World
                .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            archetype = entityManager.CreateArchetype(typeof(ClipMicroSample));
        }
        
        protected override void OnUpdate()
        {
            
            if (!GameManager.Instance.TryFetch(out IClipRegister clipRegister))
            {
                return;
            }
            
            Entities.WithStructuralChanges().WithAll<UnprocessedComponent>()
                .ForEach((Entity entity, in ClipAliasComponent clipAlias, in PlayEventComponent eventDetails) =>
            {
                if (!clipRegister.TryGetClip(clipAlias, out AudioClip clip))
                {
                    return;
                }

                float[] data = new float[clip.samples * clip.channels];
                clip.GetData(data, 0);
                
                int channel = 0;
                for (int index = 0; index < data.Length; index+=clip.channels)
                {
                    channel = (index > clip.samples - 1) ? channel + 1 : channel;
                    float amplitude = data[index] * eventDetails.Volume;
                    int samplePosition = clip.frequency * (int)eventDetails.TrackTime + index;
                    Entity microSampleEntity = entityManager.CreateEntity(archetype);
                    ClipMicroSample microSample = (clip.channels == 1)
                        ? new ClipMicroSample(data[index] * eventDetails.Volume, 
                            samplePosition, 
                            eventDetails.Pitch)
                        : new ClipMicroSample(data[index] * eventDetails.Volume,
                            data[index + 1] * eventDetails.Volume,
                            samplePosition,
                            eventDetails.Pitch);
                    entityManager.SetComponentData(microSampleEntity, microSample);
                    entityManager.RemoveComponent<UnprocessedComponent>(entity);
                }
            }).Run();
        }
    }
}