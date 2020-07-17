using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class TestSystem : SystemBase
{
    private float timeSinceStart = 0f;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        if (!AudioController.Instance.IsPlaying)
        {
            return;
        }

        float currentTimeSinceStart = timeSinceStart;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();

        Entities.WithNone<TriggeredTag, TriggerNowTag>().ForEach((Entity entity, int entityInQueryIndex, in PlayEvent playEvent) =>
        {
            if (currentTimeSinceStart >= playEvent.timeAfterStart)
            {
                ecb.AddComponent<TriggerNowTag>(entityInQueryIndex, entity);
            }
        }).ScheduleParallel();

        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        timeSinceStart += Time.DeltaTime;
    }
}

[UpdateBefore(typeof(TestSystem))]
public class AnotherSystem : SystemBase
{
    protected override void OnUpdate()
    {
        Entities.WithoutBurst().WithAll<TriggerNowTag>().ForEach((in ClipReference clip) =>
        {
            //AudioLibraryComponent.PlaySound(clip.index);
            AudioController.Instance.PlaySound(clip.index);
        }).Run();
    }
}

[UpdateBefore(typeof(AnotherSystem))]
public class OneMoreSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().ToConcurrent();
        Entities.WithAll<TriggerNowTag>().ForEach((Entity entity, int entityInQueryIndex) =>
        {
            ecb.AddComponent<TriggeredTag>(entityInQueryIndex, entity);
            ecb.RemoveComponent<TriggerNowTag>(entityInQueryIndex, entity);
        }).ScheduleParallel();
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}