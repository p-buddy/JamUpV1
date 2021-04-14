using AudioPlayBack;
using MonoBehaviours;
using Unity.Entities;
using UnityEngine;

public class StageClipsForPlaySystem : SystemBase
{
    private const double StagingTime = 1;    
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
        GameManager.Instance.TryFetch(out IPlayBack playBack);
        if (playBack == null)
        {
            return;
        }

        double currentTrackTime = playBack.CurrentTrackTime;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer().AsParallelWriter();

        Entities.WithNone<ScheduledComponent, ScheduleForPlayComponent>().ForEach((Entity entity, int entityInQueryIndex, in PlayEventComponent playEvent) =>
        {
            if (currentTrackTime >= (playEvent.TrackTime - StagingTime))
            {
                ecb.AddComponent<ScheduleForPlayComponent>(entityInQueryIndex, entity);
            }
        }).ScheduleParallel();

        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}